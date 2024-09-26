import json
from time import time
from random import random
from flask import Flask, render_template, make_response, request, jsonify

app = Flask(__name__)

temperature = 0.0
humidity = 0.0

@app.route('/')
def hello_world():

    return render_template('index.html')


@app.route('/live-data', methods=['POST'])
def live_data():
    global temperature, humidity

    #데이터 받아 오는거 구현하기
    if request.is_json:
        received_data = request.json

        # 아래처럼 하니까 연결이 끊어져도 데이터를 만든다 흠...
        temperature = received_data.get('Temperature', temperature)
        humidity = received_data.get('Humidity', humidity)
    else:
        return jsonify({'error': 'Invalid request format'}), 400

    # # # Create a PHP array and echo it as JSON
    data1 = [(time() + 32400) * 1000, temperature]
    data2 = [(time() + 32400) * 1000, humidity]
    # data1 = [time() * 1000, 32]
    # data2 = [time() * 1000, 24]

    response_data = [data1, data2]
    response = make_response(json.dumps(response_data))
    # print("Response body:", response.get_data(as_text=True))
    response.content_type = 'application/json'
    return response

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5001)

