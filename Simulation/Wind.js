let windChart;

/* formula for wind turbine farm power generation:
P = 0.5 x ρ x A x Cp x V3 
where, ρ = Air density in kg/m3
    A = Rotor swept area (m2) 
    Cp = Coefficient of performance 
    V = wind velocity (m/s) */

const WIND_POWER_GENERATION_FORMULA = 0.5 * 1.21 * 7853.98163 * 0.6            


// navigating to solar page
function navigateToSolar() {
    window.location.href = "solar.html";
}

// loading data from CSV and populating graph and table                        
document.addEventListener("DOMContentLoaded", () => {
    fetchData("/Data/WindSpeedData.csv", processWindData, populateWindTable, createWindChart);
});

// creating callback functions to retrieve, process and display data                                      
function fetchData(csvFile, processDataCallback, populateTableCallback, createChartCallback) {
    fetch(csvFile)
        .then(response => response.text())
        .then(data => {
            let rows = data.split("\n").slice(1);
            let dailyData = processDataCallback(rows);
            populateTableCallback(dailyData);
            createChartCallback(dailyData);
        });
}

// calculating daily MW generation
function processWindData(rows) {
    let dailyData = {};
    rows.forEach(row => {
        let [date, actualWindSpeed, predictedWindSpeed] = row.split(",");
        let actualKW = (WIND_POWER_GENERATION_FORMULA * Math.pow(parseFloat(actualWindSpeed), 3)) / 100000;
        let predictedKW = (WIND_POWER_GENERATION_FORMULA * Math.pow(parseFloat(predictedWindSpeed), 3)) / 100000;

        if (!dailyData[date]) {
            dailyData[date] = { actual: 0, predicted: 0, count: 0 };
        }

        dailyData[date].actual += actualKW;
        dailyData[date].predicted += predictedKW;
        dailyData[date].count += 1;
    });

    // calcuating daily average
    for (const date in dailyData) {
        dailyData[date].actual /= dailyData[date].count;
        dailyData[date].predicted /= dailyData[date].count;
    }

    return dailyData;
}

// populating wind data table
function populateWindTable(data) {
    const tableBody = document.getElementById("windDataTable").querySelector("tbody");
    tableBody.innerHTML = "";
    for (const [date, { actual, predicted }] of Object.entries(data)) {
        const row = `<tr><td>${date}</td><td>${actual.toFixed(2)} </td><td>${predicted.toFixed(2)} </td></tr>`;
        tableBody.insertAdjacentHTML("beforeend", row);
    }
}

// creating line graph with data
function createWindChart(dailyData) {
    const labels = Object.keys(dailyData);
    const actualData = labels.map(date => dailyData[date].actual);
    const predictedData = labels.map(date => dailyData[date].predicted);

    const ctx = document.getElementById("windChart").getContext("2d");
    windChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels,
            datasets: [
                { label: 'Actual Wind Power Production (MW)', data: actualData, borderColor: 'blue', fill: false },
                { label: 'Predicted Wind Power Production (MW)', data: predictedData, borderColor: 'gray', fill: false }
            ]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: true, position: 'top' }
            }
        }
    });
}

// aggregating daily data into monthly averages
/* function aggregateMonthlyData(dailyData) {
    const monthlyData = {};
    for (const date in dailyData) {
        const month = date.slice(0, 7);         // in the format of yyyy-mm
        if (!monthlyData[month]) {
            monthlyData[month] = { actual: 0, predicted: 0, count: 0 };
        }
        monthlyData[month].actual += dailyData[date].actual;
        monthlyData[month].predicted += dailyData[date].predicted;
        monthlyData[month].count += 1;
    }

    // calculating monthly averages
    for (const month in monthlyData) {
        monthlyData[month].actual /= monthlyData[month].count;
        monthlyData[month].predicted /= monthlyData[month].count;
    }
    return monthlyData;
} */