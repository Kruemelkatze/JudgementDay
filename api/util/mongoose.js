import mongoose from 'mongoose';


const { MONGODB_MONGOOSE_URI } = process.env

const Schema = mongoose.Schema;
const ObjectId = Schema.Types.ObjectId;


export const getModels = async() => {
    if (typeof global.mongodb == 'undefined') {
        global.models = [];
        console.log('connect')
        global.mongodb = await mongoose.connect(MONGODB_MONGOOSE_URI, {
            useNewUrlParser: true,
            useUnifiedTopology: true,
        });
        console.log(global.mongodb)

        const StatsSchema = new Schema({
            _id: ObjectId,
            name: String, // name of the judged person
            user: String, // name of the user
            createdAt: Date,
            score: Number
        });
        
        const Stats = mongoose.model('Stats', StatsSchema);
        global.models['stats'] = Stats;

    }

    console.log(global.models);
    return global.models;
}

// db = global.mongodb;

module.exports = {
    getModels
}