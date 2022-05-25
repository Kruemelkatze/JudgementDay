// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type { NextApiRequest, NextApiResponse } from 'next'
import NextCors from 'nextjs-cors';
import { getAllStats } from '../../helper/dbHelper'

type Data = {
  stats: any,
  success: boolean,
  time: number
}

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse<Data>
) {

  let mapArray: any[any] = {};
  (await getAllStats()).map((s: { name: string, score: number }) => {
    if (s.score) {
      if (mapArray[s.name]) {
        mapArray[s.name] = [mapArray[s.name][0] + 1, mapArray[s.name][1] + s.score];
      }
      else {
        mapArray[s.name] = [1, s.score];
      }
    }
  });

  // mapArray = mapArray.map((e: number[]) => e[1] / e[0]);
  for (let e in mapArray) {
    mapArray[e] = mapArray[e][1] / mapArray[e][0];
  }

  await NextCors(req, res, {
    // Options
    methods: ['GET'],
    origin: '*',
    optionsSuccessStatus: 200, // some legacy browsers (IE11, various SmartTVs) choke on 204
  });

  res.status(200).json({ stats: mapArray, time: Date.now(), success: true })
}
