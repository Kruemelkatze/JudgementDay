// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type { NextApiRequest, NextApiResponse } from 'next'
import NextCors from 'nextjs-cors'
import { createStat } from '../../helper/dbHelper'

type Data = {
  message: string,
  success: boolean,
  time: number
}

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse<Data>
) {

  if (req.method !== 'POST' || !req.body) {
    res.status(405).json({ message: 'Only POST requests allowed', success: false, time: Date.now() })
    return;
  }

  const body = req.body;

  if (!(body.name && body.score && body.user)) {
    res.status(405).json({ message: 'name or score or user not defined in request body', success: false, time: Date.now() })
    return;
  }

  await createStat(body.name, body.user, body.score);

  await NextCors(req, res, {
    // Options
    methods: ['POST'],
    origin: '*',
    optionsSuccessStatus: 200, // some legacy browsers (IE11, various SmartTVs) choke on 204
 });

  return res.status(200).json({ success: true, time: Date.now(), message: '' })
}
