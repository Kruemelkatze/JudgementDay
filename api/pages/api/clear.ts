// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import type { NextApiRequest, NextApiResponse } from 'next'
import { clear } from '../../helper/dbHelper'
const { PASSWORD } = process.env

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

  if (!(body.password)) {
    res.status(405).json({ message: 'password not defined in request body', success: false, time: Date.now() })
    return;
  }

  if (body.password != PASSWORD) {
    res.status(405).json({ message: 'password wrong', success: false, time: Date.now() })
    return;
  }

  await clear();

  return res.status(200).json({ success: true, time: Date.now(), message: '' })
}
