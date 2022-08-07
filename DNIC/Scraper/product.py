class Product:
    def __init__(self, name, productType, price, url, realId):
        self.name = name
        self.productType = productType
        self.price = price
        self.url = url
        self.realId = realId

    def getData(self):
        return {
            "name": self.name,
            "type": self.productType,
            "price": self.price,
            "url": self.url,
            "realId": self.realId,
        }