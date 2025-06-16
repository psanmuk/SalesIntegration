const express = require('express');
const knex = require('knex')({
  client: 'mysql2',
  connection: {
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'yourdb'
  }
});
const app = express();

app.get('/api/tb_trans', async (req, res) => {
  const { saledate, spid } = req.query;
  const data = await knex('tb_trans').where({ SaleDate: saledate, SPID: spid });
  res.json(data);
});

app.listen(3000, () => console.log('API running on port 3000'));
