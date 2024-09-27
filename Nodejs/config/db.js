require('dotenv').config();
const { MongoClient } = require('mongodb');
const mongoURI = process.env.MONGO_URI;

if (!mongoURI) {
    throw new Error('MONGO_URI non défini dans le fichier .env');
}

const client = new MongoClient(mongoURI);

let db;

async function connectToDatabase() {
    if (!db) {
        await client.connect();
        db = client.db('imagetransform');
        console.log('Connected to MongoDB');
    }
    return db;
}

module.exports = connectToDatabase;
