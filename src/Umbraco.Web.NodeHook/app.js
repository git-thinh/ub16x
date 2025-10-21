const express = require("express");
const app = express();
const port = 59731;

app.get("/", (req, res) => res.send("OK"));

app.post("/", (req, res) => {
  const data = req.body;
  console.log(data)
  res.status(201).json({ message: "", data });
});

app.listen(port, () => console.log(`http://localhost:${port}`));
