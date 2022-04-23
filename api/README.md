JudgementDay API

## Development

First, run the development server:

```bash
npm run dev
# or
yarn dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

## Api paths

### get list of all stats data
https://judgementdayapi.vercel.app/api/list

### get all stats
https://judgementdayapi.vercel.app/api/stats

### add score entry
```bash
curl -d '{"user": "me", "name": "person1", "score": 1}' -H 'Content-Type: application/json' -X POST https://judgementdayapi.vercel.app/api/addStat
```