var chart;

/**
 * Request data from the server, add it to the graph and set a timeout
 * to request again
 */

function requestData() {
    $.ajax({
        url: '/live-data',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({}),
        success: function(point) {
            console.log("Response received:", point);

            var series = chart.series[0],
                            shift = series.data.length > 20;

            // add the point
            chart.series[0].addPoint(point[0], true, shift);
            chart.series[1].addPoint(point[1], true, shift);


            // call it again after one second
            setTimeout(requestData, 1000);
        },
        cache: false
    });

}

$(document).ready(function() {
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'data-container',
            defaultSeriesType: 'spline',
            events: {
                load: requestData
            }
        },
        title: {
            text: '온습도 측정 데이터'
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 150,
            maxZoom: 20 * 1000
        },
        yAxis: {
            minPadding: 0.2,
            maxPadding: 0.2,
            title: {
                text: '온습도',
                margin: 80
            }
        },
        series: [{
            name: '온도',
            data: []
        },{
            name: '습도',
            data: []
        }]
    });
});