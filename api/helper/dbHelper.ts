import mongoose from 'mongoose';
import { getModels } from '../util/mongoose';

const reserialize = (object: any) => {
    return JSON.parse(JSON.stringify(object));
}

export const getAllStats = async () => {
    const models = await getModels();
    const Stats = models['stats'];
    const stats = await Stats.find({});
    return reserialize(stats);
}

export const createStat = async (name: String, user: String, score: Number) => {
    const models = await getModels();
    const Stats = models['stats'];
    const instance = new Stats();

    instance._id = new mongoose.Types.ObjectId();
    instance.name = name;
    instance.user = user;
    instance.score = score;
    instance.createdAt = Date.now();
    instance.save(function (err: any) {
        console.log(err);
    });
}

export const clear = async () => {
    const models = await getModels();
    const Stats = models['stats'];
    
    await Stats.remove({});
}


module.exports = {
    getAllStats,
    createStat,
    clear
}