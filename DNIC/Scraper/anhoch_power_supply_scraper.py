from math import prod
from operator import contains
from bs4 import BeautifulSoup
import requests

from product import Product

# example url https://anhoch.com/category/376/matichni-plochi' + '%23' + 'stock/2/page/1/

hashtag_character = '%23'
url = 'https://anhoch.com/category/388/napojuvanja' + '%23' + 'stock/1/page/'

apiUrl = "https://localhost:44376/api/ProductApi"

products = []

i=1
while True:
    page = requests.get(f"{url}{i}/")

    soup = BeautifulSoup(page.content, 'html.parser')
    lists = soup.find_all('li', class_="product-fix")
    if(len(lists) == 0):
        break

    for product in lists:
        if(product.select_one('.pull-right a:first-child') is None):
            continue

        productNameTag = product.select_one('.product-name a:first-child')
        name = productNameTag.text
        productUrl = productNameTag['href']
        productType = 'Power Supply'

        priceString = product.select_one('.nm').text
        numeric_filter = filter(str.isdigit, priceString)
        numeric_string = "".join(numeric_filter)
        price = numeric_string

        realId = product['data-id']

        productEntity = Product(name=name, productType=productType, url=productUrl, price=price, realId=realId)
        products.append(productEntity)

    i = i+1

for product in products:
    # from pprint import pprint
    # pprint(product.getData())

    requests.post(url=apiUrl, data=product.getData(), verify=False)

    


