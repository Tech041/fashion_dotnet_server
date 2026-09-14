# EcommerceServer

A robust backend API for managing products, users, and visitor tracking in an e‑commerce application. Built with **ASP.NET Core**, integrated with **PostgreSQL (Neon)** for persistence, and **Cloudinary** for image storage and optimization.

---

##  Features

- **Product Management**
  - Upload product with image compression (WebP format)
  - Fetch all products or by collection
  - Get product details by slug
  - Update and delete products

- **User Authentication**
  - Register new users
  - Login with JWT‑based authentication

- **Visitor Tracking**
  - Track visits by visitor ID
  - Retrieve visitor statistics

---

##  Tech Stack

- **Backend Framework**: ASP.NET Core Web API  
- **Database**: PostgreSQL (via EF Core, hosted on Neon)  
- **Image Storage**: Cloudinary (with ImageSharp for preprocessing)  
- **Authentication**: JWT  
- **Documentation**: Swagger  

---

##  Setup & Installation

1. **Clone the repository**
   ```bash
   git clone git@github.com:Tech041/fashion_dotnet_server.git
   cd EcommerceServer

2. **Configure secrets**
   
	dotnet user-secrets set "Jwt:Key" "your-secret-key"			
	dotnet user-secrets set "Cloudinary:CloudName" "your-cloud-name"
	dotnet user-secrets set "Cloudinary:ApiKey" "your-api-key"
	dotnet user-secrets set "Cloudinary:ApiSecret" "your-api-secret"
	dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-postgres-url"

		
	Production (Render): set environment variables with __ instead of :  
    Example: Cloudinary__ApiKey, Jwt__Key, ConnectionStrings__DefaultConnection

3. **Run migrations**
    
   dotnet ef database update

4. **Start the server**
   dotnet run


   
### API Endpoints
Product
POST /api/product/upload → Upload a new product with image

GET /api/product/all-products → Fetch all products

GET /api/product/collection → Fetch products by collection

GET /api/product/{slug} → Fetch product details by slug

PATCH /api/product/update/{id} → Update product by ID

DELETE /api/product/delete/{id} → Delete product by ID


User
POST /api/user/register → Register a new user (Deactivated user registration UI but endpoint is active and works on Swagger)

POST /api/user/login → Login and receive JWT

VisitorTracker
POST /api/visit/track/{visitorId} → Track a visitor by ID

GET /api/visit/stats → Get visitor statistics

### Data Models
CreateUser

Login

Product (with sizes, slug, image URL, etc.)

VisitorStat / DailyVisit

## Development Notes
Images are resized to max 800×800 and compressed to WebP (lossy, quality 60) before upload.

Cloudinary stores images in the dotnetproducts folder.

JWT authentication secures protected endpoints.

Swagger UI is available at: http://localhost:5156/swagger/index.html


### License
This project is licensed under the MIT License.

