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

  const stats = await getAllStats();

  await NextCors(req, res, {
    // Options
    methods: ['GET'],
    origin: '*',
    optionsSuccessStatus: 200, // some legacy browsers (IE11, various SmartTVs) choke on 204
  });

  res.status(200).json({ success: true, stats, time: Date.now() })
}
