require('dotenv').config(); // Charge les variables d'environnement depuis .env

const express = require('express');
const app = express();
const port = process.env.PORT || 3000;
const host = process.env.HOST || '127.0.0.1';

const mongoRoutes = require('./routes/mongo');
const image = require('./routes/image');

app.use(express.json());

app.use('/api', mongoRoutes);
app.use('/imagetransform', image);

app.get('/', (req, res) => {
    res.send('Hello World!');
});

app.listen(port, host, () => {
    console.log(`App listening at http://${host}:${port}`);
});