// 1. Enum для категорий товаров
var ProductCategory;
(function (ProductCategory) {
    ProductCategory["Electronics"] = "Electronics";
    ProductCategory["Clothing"] = "Clothing";
    ProductCategory["Books"] = "Books";
    ProductCategory["Food"] = "Food";
    ProductCategory["Other"] = "Other";
})(ProductCategory || (ProductCategory = {}));
// 2. Класс Product
class Product {
    constructor(id, name, price, category, description) {
        this.id = id;
        this.name = name;
        this.price = price;
        this.category = category;
        this.description = description;
    }
    getInfo() {
        return `ID: ${this.id}, Name: ${this.name}, Price: ${this.price}, Category: ${this.category}, Description: ${this.description || 'N/A'}`;
    }
}
// 3. Класс Catalog
class Catalog {
    constructor() {
        this.products = [];
        this.products = [];
    }
    addProduct(product) {
        this.products.push(product);
    }
    removeProduct(id) {
        this.products = this.products.filter(p => p.id !== id);
    }
    getProductById(id) {
        return this.products.find(p => p.id === id);
    }
    getAllProducts() {
        return this.products;
    }
    getProductsByCategory(category) {
        return this.products.filter(p => p.category === category);
    }
}
// 4. Дженерик-класс Order
class Order {
    constructor(id, products) {
        this.id = id;
        this.products = products;
        this.totalPrice = this.calculateTotalPrice();
    }
    calculateTotalPrice() {
        return this.products.reduce((sum, product) => sum + product.price, 0);
    }
    getOrderInfo() {
        const productList = this.products.map(p => p.name).join(', ');
        return `Order ID: ${this.id}, Products: [${productList}], Total Price: ${this.totalPrice}`;
    }
}
// 5. Класс Customer
class Customer {
    constructor(id, name, email) {
        this.id = id;
        this.name = name;
        this.email = email;
    }
    getCustomerInfo() {
        return `Customer ID: ${this.id}, Name: ${this.name}, Email: ${this.email}`;
    }
}
// 6. Класс OrderManager
class OrderManager {
    constructor() {
        this.orders = [];
        this.orders = [];
    }
    createOrder(customer, products) {
        const orderId = this.orders.length + 1; // простой способ генерации ID
        const newOrder = new Order(orderId, products);
        this.orders.push(newOrder);
        return newOrder;
    }
    getOrderById(id) {
        return this.orders.find(order => order.id === id);
    }
    getAllOrders() {
        return this.orders;
    }
    getOrdersByCustomer(customerId) {
        return [];
    }
}
const catalog = new Catalog();
const product1 = new Product(1, 'Laptop', 1200, ProductCategory.Electronics, 'High performance laptop');
const product2 = new Product(2, 'T-Shirt', 25, ProductCategory.Clothing, 'Cotton T-Shirt');
const product3 = new Product(3, 'JavaScript Book', 30, ProductCategory.Books);
const order2 = new Order(1, [product1, product2]);
// Добавление товаров в каталог
catalog.addProduct(product1);
catalog.addProduct(product2);
catalog.addProduct(product3);
console.log(catalog.getProductsByCategory(ProductCategory.Electronics));
const customer = new Customer(1, 'Vadim Potrebko', 'vadim26129@gmail.com');
const orderManager = new OrderManager();
const order = orderManager.createOrder(customer, [product1, product3]);
const order1 = orderManager.getOrderById(1);
const order22 = orderManager.getAllOrders();
const order3 = orderManager.getOrdersByCustomer(1);
console.log("ORDER", order);
console.log("ORDER1", order1);
console.log("ORDER22", order22);
console.log("ORDER3", order3);
const partial = { name: 'Partial Product' };
const withoutId = {
    name: 'No ID',
    price: 100,
    category: ProductCategory.Other,
    getInfo: () => ''
};
const orderSummary = { id: order.id, totalPrice: order.totalPrice };
console.log('Order summary:', orderSummary);
