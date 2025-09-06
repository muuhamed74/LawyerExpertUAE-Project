# 📄 Lawyer Expert UAE – Backend

## 🔹 Overview  
This backend powers the **Lawyer Expert UAE** platform, a system designed to help users manage and customize legal contracts.  
The main concept:  
- Users can **sign up and log in** securely.  
- They can **view and edit pre-defined legal contract templates**.  
- Every modification creates a **personal copy** of the contract linked to the user’s account.  
- Users can return later to review and manage all contracts they have customized.

The project ensures secure authentication, contract storage, and efficient data handling.

🔗 **Live Project:** [Law Experts UAE](https://lawexpertsuae.com/home)
---

## 🚀 Features  
- **Authentication & Authorization**  
  - User registration with email verification (OTP).  
  - Secure login with refresh tokens.  
  - Role-based access control (Admin/User).  
  - Password reset via OTP.  

- **Contract Management**  
  - Browse predefined legal contract templates.  
  - Upload and save customized versions.  
  - Retrieve user-specific contracts at any time.  

- **Security & Deployment**  
  - HTTPS enabled with **Let’s Encrypt SSL certificates**.  
  - Reverse proxy and load handling with **Nginx**.  
  - Deployed inside **Docker containers** for scalability.  

---

## 🛠 Tech Stack  
- **Framework**: .NET Core (C#)  
- **Database**: PostgreSQL (Entity Framework Core for ORM)  
- **Authentication**: ASP.NET Identity + JWT + Refresh Tokens  
- **Deployment**:  
  - **Nginx** (reverse proxy & static file serving)  
  - **Docker** (containerized backend)  
  - **Certbot** (SSL certificate management)  

---

## 📂 Key Components  
- **ContractsController**  
  - Provides endpoints to retrieve templates.  
  - Allows users to upload their own customized contracts.  
  - Supports fetching all user-specific contracts.  

- **AccountsController**  
  - Handles user signup with OTP verification.  
  - Manages login, logout, and token refresh.  
  - Provides password reset via OTP.  
  - Includes role management for administrators.  

---


⚡ This project demonstrates my ability to build a **secure, scalable, and production-ready backend** with .NET, PostgreSQL, and modern deployment tools.
