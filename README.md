# SalesIntegration

ตัวอย่างโปรเจ็กต์ VB.NET + Node.js API + SQL  
**สำหรับ Visual Studio 2017 + Node.js + MySQL + SQL Server**

## โครงสร้าง
- `VBNetApp/` : แอป VB.NET เรียก API และรัน Stored Procedure
- `NodeAPI/` : Node.js API + Knex
- `SQLScripts/` : Stored procedure และ SQL ที่ใช้

## วิธีรัน
### Node.js
```bash
cd NodeAPI
npm install
node server.js

# SalesIntegration Project

โปรเจ็กต์นี้ทำหน้าที่ดึงข้อมูลรายการขายจาก API (Node.js + MySQL) แล้ว merge เข้ากับฐานข้อมูล Microsoft SQL Server ผ่าน VB.NET (.NET Framework 4.5)

## โครงสร้างโปรเจ็กต์
## การติดตั้ง

### Node.js API

1. ติดตั้ง Node.js + npm
2. ติดตั้ง dependency:
    ```bash
    npm install express mysql2 knex
    ```
3. แก้ไขการเชื่อมต่อ DB ใน `NodeAPI/app.js`
4. เริ่ม API:
    ```bash
    node NodeAPI/app.js
    ```

### VB.NET

1. เปิด Visual Studio 2017
2. สร้าง Console App (.NET Framework 4.5) แล้วนำ `Program.vb` ไปวาง
3. เพิ่ม reference:
   - System.Net.Http
   - System.Data
4. แก้ไข connection string ใน `Program.vb` ให้ตรงกับ MS SQL Server ของคุณ

### SQL Server

1. สร้าง Stored Procedure โดยรันไฟล์ `SQL/usp_MergeTransData.sql`

## การใช้งาน

1. รัน Node.js API
2. รัน VB.NET Console App
3. โปรแกรมจะแจ้งสถานะ (เช่น กำลังโหลดข้อมูล, สำเร็จ, หรือ error)

## หมายเหตุ

- ตัวอย่างนี้ใช้พารามิเตอร์ hardcode สามารถปรับให้รับจาก input ได้
- โปรดตรวจสอบสิทธิ์ในการเขียนข้อมูล SQL Server

## License

MIT
---

## 🚀 **ขั้นตอนต่อไป**
1️⃣ สร้างโครงสร้างโฟลเดอร์ตามด้านบน  
2️⃣ วางไฟล์เนื้อหาตามที่ให้  
3️⃣ GitHub:  
```bash
git init
git remote add origin https://github.com/yourusername/SalesIntegration.git
git add .
git commit -m "Initial commit"
git push -u origin master

