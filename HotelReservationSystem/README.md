# Hotel Reservation System

## Installation
1. **Clone the repository**.
2. **Database Setup**: 
   - Open **Package Manager Console** in Visual Studio.
   - Run command: `Update-Database`.
3. **Connection String**: in `appsettings.json` under `DefaultConnection`.

## Test Users
We have 2 roles, **User** and **Admin**:

| Role | Email | Password | Permissions |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@hotel.com` | `Admin123!` | Edit, Delete, Manage Rooms/Guests |
| **User** | `user@hotel.com` | `User123!` | Browse Rooms, Add Reservations |

## Application Modules
* **Rooms & Room Types**: Admin-only management of the hotel inventory.
* **Guests**: Management of guest profiles (linked to reservations).
* **Reservations**: Form with date validation for booking rooms.
* **API CRUD**: REST API for the Room entity located at `/api/rooms`.

## Entities and Relationships
1. **Room**: Main entity for hotel.
2. **RoomType**: Linked to Room (One-to-Many).
3. **Guest**: Represents the customer.
4. **Reservation**: Links Guest and Room (Many-to-Many relationship).