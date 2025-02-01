const express = require('express');
const bodyParser = require('body-parser');

const app = express();
const PORT = 5000;

// Middleware to parse JSON bodies
app.use(bodyParser.json());

// Handle POST request
app.post('/api/data', (req, res) => {

    const data = req.body; // Access the data sent in the request body
    console.log('Received data:', data);

    // Respond with a success message
    res.json({
        message: 'Data received successfully!',
        receivedData: data
    });
});

// Start the server
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
