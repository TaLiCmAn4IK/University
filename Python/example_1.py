from requests import get

address = input()
total_sum = 0

while True:
    response = get(f'http://{address}/')
    numbers = [x for x in response.json() if isinstance(x, int)]
    total_sum += numbers
print(numbers)