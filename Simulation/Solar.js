let solarChart;

/* formula for solar farm power generation: 
p = 104*(UV Index) – 18.365 */
const SOLAR_POWER_GENERATION_FORMULA = -18.365

// navigating to wind page
function navigateToWind() {
    window.location.href = "wind.html";
}

// loading data from CSV and populating graph and table
document.addEventListener("DOMContentLoaded", () => {
    fetchData("/Data/UVIndexData.csv", processSolarData, populateSolarTable, createSolarChart);
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

// calculating daily kWh generation
function processSolarData(rows) {
    let dailyData = {};
    rows.forEach(row => {
        let [date, actualUV, predictedUV] = row.split(",");   
        let actualKW = ((parseFloat(actualUV) * 104) + SOLAR_POWER_GENERATION_FORMULA) * 8.4 / 100;
        let predictedKW = ((parseFloat(predictedUV) * 104) +  SOLAR_POWER_GENERATION_FORMULA) * 8.4 / 100;

        if (!dailyData[date]) {
            dailyData[date] = { actual: 0, predicted: 0, count: 0 };
        }

        dailyData[date].actual += actualKW;
        dailyData[date].predicted += predictedKW;
        dailyData[date].count += 1;       
    });

    // calcuating daily averages
    for (const date in dailyData) {
        dailyData[date].actual /= dailyData[date].count;
        dailyData[date].predicted /= dailyData[date].count;
    }

    return dailyData;
}

// populating solar data table 
function populateSolarTable(data) {
    const tableBody = document.getElementById("solarDataTable").querySelector("tbody");
    tableBody.innerHTML = "";
    for (var [date, { actual, predicted }] of Object.entries(data)) {
        let row = `<tr><td>${date}</td><td>${actual.toFixed(2)} </td><td>${predicted.toFixed(2)} </td></tr>`;
        tableBody.insertAdjacentHTML("beforeend", row);
    }
}

// creating chart
function createSolarChart(dailyData) {
    const labels = Object.keys(dailyData);
    const actualData = labels.map(date => dailyData[date].actual);
    const predictedData = labels.map(date => dailyData[date].predicted);

    const ctx = document.getElementById("solarChart").getContext("2d");
    solarChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels,
            datasets: [
                { label: 'Actual Solar Power Production (MW)', data: actualData, borderColor: 'orange', fill: false },
                { label: 'Predicted Solar Power Production (MW)', data: predictedData, borderColor: 'gray', fill: false }
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
        const month = date.slice(0, 7); // in yyyy-mm format
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