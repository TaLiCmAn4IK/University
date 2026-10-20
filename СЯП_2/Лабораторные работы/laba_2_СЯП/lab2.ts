// 1. Enum для категорий товаров
enum ProductCategory {
  Electronics = "Electronics",
  Clothing = "Clothing",
  Books = "Books",
  Food = "Food",
  Other = "Other"
}

// 2. Класс Product
class Product {
  public readonly id: number; 
  public name: string;
  public price: number;
  public description?: string; 
  public category: ProductCategory;

  constructor(id: number, name: string, price: number, category: ProductCategory, description?: string) {
    this.id = id;
    this.name = name;
    this.price = price;
    this.category = category;
    this.description = description;
  }

  public getInfo(): string {
    return `ID: ${this.id}, Name: ${this.name}, Price: ${this.price}, Category: ${this.category}, Description: ${this.description || 'N/A'}`;
  }
}

type ProductWithoutId = Omit<Product, 'id'>; 
type PartialProduct = Partial<Product>; 

// 3. Класс Catalog
class Catalog {
  private products: Product[] = []; 

  constructor() {
    this.products = [];
  }

  public addProduct(product: Product): void { 
    this.products.push(product);
  }

  public removeProduct(id: number): void {
    this.products = this.products.filter(p => p.id !== id);
  }

  public getProductById(id: number): Product | undefined {  
    return this.products.find(p => p.id === id);
  }

  public getAllProducts(): Product[] {
    return this.products;
  }

  public getProductsByCategory(category: ProductCategory): Product[] {
    return this.products.filter(p => p.category === category);
  }
}

// 4. Дженерик-класс Order
class Order<T extends Product> {
  public readonly id: number;
  public products: T[];
  public totalPrice: number;

  constructor(id: number, products: T[]) {
    this.id = id;
    this.products = products;
    this.totalPrice = this.calculateTotalPrice();
  }

  public calculateTotalPrice(): number {
    return this.products.reduce((sum, product) => sum + product.price, 0);
  }

  public getOrderInfo(): string {
    const productList = this.products.map(p => p.name).join(', ');
    return `Order ID: ${this.id}, Products: [${productList}], Total Price: ${this.totalPrice}`;
  }
}

type OrderSummary = Pick<Order<Product>, 'id' | 'totalPrice'>;

// 5. Класс Customer
class Customer {
  public readonly id: number;
  public name: string;
  public email: string;

  constructor(id: number, name: string, email: string) {
    this.id = id;
    this.name = name;
    this.email = email;
  }

  public getCustomerInfo(): string {
    return `Customer ID: ${this.id}, Name: ${this.name}, Email: ${this.email}`;
  }
}

// 6. Класс OrderManager
class OrderManager {
  private orders: Order<Product>[] = [];

  constructor() {
    this.orders = [];
  }

  public createOrder(customer: Customer, products: Product[]): Order<Product> {
    const orderId = this.orders.length + 1; // простой способ генерации ID
    const newOrder = new Order(orderId, products);
    this.orders.push(newOrder);
    return newOrder;
  }

  public getOrderById(id: number): Order<Product> | undefined {
    return this.orders.find(order => order.id === id);
  }

  public getAllOrders(): Order<Product>[] {
    return this.orders;
  }

  public getOrdersByCustomer(customerId: number): Order<Product>[] {
    return []; 
  }
}

const catalog = new Catalog();

const product1 = new Product(1, 'Laptop', 1200, ProductCategory.Electronics, 'High performance laptop');
const product2 = new Product(2, 'T-Shirt', 25, ProductCategory.Clothing, 'Cotton T-Shirt');
const product3 = new Product(3, 'JavaScript Book', 30, ProductCategory.Books);

const order2 = new Order(1, [product1,product2]);
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
console.log("ORDER1",order1);
console.log("ORDER22",order22);
console.log("ORDER3",order3);

const partial: PartialProduct = { name: 'Partial Product' }; 

const withoutId: ProductWithoutId = { 
  name: 'No ID', 
  price: 100, 
  category: ProductCategory.Other,
  getInfo: () => '' 
};

const orderSummary: OrderSummary = { id: order.id, totalPrice: order.totalPrice };
console.log('Order summary:', orderSummary);