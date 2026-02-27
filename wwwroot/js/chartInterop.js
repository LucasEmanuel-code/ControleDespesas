(function () {
    // Removed the early return to allow updating the object

    const charts = {};

    function generateColors(n) {
        const palette = [
            '#4e79a7', '#f28e2b', '#e15759', '#76b7b2', '#59a14f', '#edc949', '#af7aa1', '#ff9da7', '#9c755f', '#bab0ac'
        ];
        const out = [];
        for (let i = 0; i < n; i++) out.push(palette[i % palette.length]);
        return out;
    }

    function renderPieChart(canvasId, labels, values, options) {
      console.log('renderPieChart called for', canvasId, 'with labels:', labels, 'values:', values);
        try {
            destroyChart(canvasId);
            const canvas = document.getElementById(canvasId);
            if (!canvas || !canvas.parentNode) {
                console.log('Canvas not found or not in DOM for', canvasId);
                return;
            }
            const ctx = canvas.getContext('2d');

            const cfg = {
                type: 'pie',
                data: {
                    labels: labels || [],
                    datasets: [{
                        data: values || [],
                        backgroundColor: generateColors((values || []).length)
                    }]
                },
                options: Object.assign({
                    responsive: true,
                    plugins: { legend: { position: 'bottom' } }
                }, options || {})
            };

            try {
                console.log('Pie chart created for', canvasId);
                charts[canvasId] = new Chart(ctx, cfg);
            } catch (chartError) {
                console.error('Error creating pie chart:', chartError);
            }
        }
        catch (e) {
            console.error('chartInterop.renderPieChart error', e);
        }
    }

    function updatePieChart(canvasId, labels, values) {
        try {
            const chart = charts[canvasId];
            if (!chart) return renderPieChart(canvasId, labels, values);
            if (!chart.canvas || !chart.canvas.parentNode) {
                delete charts[canvasId];
                return renderPieChart(canvasId, labels, values);
            }
            chart.data.labels = labels || [];
            chart.data.datasets[0].data = values || [];
            chart.update();
        }
        catch (e) { console.error('chartInterop.updatePieChart error', e); }
    }

    function renderBarChart(canvasId, labels, values, options) {
        try {
            destroyChart(canvasId);
            const canvas = document.getElementById(canvasId);
            if (!canvas || !canvas.parentNode) return;
            const ctx = canvas.getContext('2d');

            const cfg = {
                type: 'bar',
                data: {
                    labels: labels || [],
                    datasets: [{
                        data: values || [],
                        backgroundColor: generateColors((values || []).length)
                    }]
                },
                options: Object.assign({
                    responsive: true,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true }
                    }
                }, options || {})
            };

            try {
                charts[canvasId] = new Chart(ctx, cfg);
            } catch (chartError) {
                console.error('Error creating bar chart:', chartError);
            }
        }
        catch (e) {
            console.error('chartInterop.renderBarChart error', e);
        }
    }

    function destroyChart(canvasId) {
        try {
            const chart = charts[canvasId];
            if (chart) {
                try {
                    chart.destroy();
                } catch (destroyError) {
                    console.error('Error destroying chart:', destroyError);
                }
                delete charts[canvasId];
            }
        }
        catch (e) { console.error('chartInterop.destroyChart error', e); }
    }

    function canvasExists(canvasId) {
        return !!document.getElementById(canvasId);
    }

    if (!window.chartInterop) {
        window.chartInterop = {};
    }

    window.chartInterop.renderPieChart = renderPieChart;
    window.chartInterop.renderBarChart = renderBarChart;
    window.chartInterop.updatePieChart = updatePieChart;
    window.chartInterop.destroyChart = destroyChart;
    window.chartInterop.canvasExists = canvasExists;

    console.log('chartInterop loaded', window.chartInterop);
})();
