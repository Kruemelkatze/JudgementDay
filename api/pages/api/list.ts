// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type { NextApiRequest, NextApiResponse } from 'next'
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
  res.status(200).json({ success: true, stats, time: Date.now() })
}
