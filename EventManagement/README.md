# Eventify - Event Management System
[Visit the Website](https://eventmanagementsys.azurewebsites.net/)
## Abstract

Eventify is a comprehensive Event Management System developed using ASP.NET MVC Core, Entity Framework, and SQL Server. It provides users with a versatile platform to create, manage, and participate in various events. With a focus on user engagement and administrative efficiency, Eventify offers a seamless experience for event organizers and participants alike.

## Key Features

1. **User Registration and Authentication**: Secure account creation and authentication mechanisms ensure a safe and protected user experience.
   
2. **Private Event Creation**: Registered users can organize their private events, customizing details such as event name, date, venue, and additional services.
   
3. **Admin Event Creation**: Administrators have the authority to create and manage admin events, facilitating ticket purchases and event coordination.
   
4. **Dashboard Functionality**: Both users and administrators benefit from personalized dashboards, providing insights into registered events, ticket purchases, and event management tools.
   
5. **Event Participation**: Users can explore and participate in a diverse range of upcoming events, spanning various categories and interests.
   
Eventify aims to streamline event management processes, enhancing user engagement and administrative effectiveness.

## Project Goals

- Establish a user-friendly online platform for event enthusiasts to create and participate in events seamlessly.
  
- Digitize and optimize event management operations, improving the overall experience for organizers and participants.
  
- Ensure data security and confidentiality throughout the system's operation.

## Database Schema and Dataflow Diagram

- **Database Schema**:![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/1e6363ef-ca63-414f-98a8-1faa518c48d2)

- **Dataflow Diagram**:![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/129a376e-4570-48ac-b987-d26c81428a21)

## Entity Relationship

1. **One-to-Many Relationship**:
   - Between Events and Halls: Each event can be associated with one hall, facilitating venue management and event organization.

2. **Many-to-Many Relationship**:
   - Between Users and Roles: Users can have multiple roles, enabling flexible access control and permission management.

3. **One-to-Many Relationship (Bidirectional)**:
   - Between Events and Tickets: Events can have multiple tickets, allowing for efficient ticket management and sales tracking.

4. **Many-to-Many Relationship (Bidirectional)**:
   - Between Users and Events: Users can participate in multiple events, fostering community engagement and interaction.

## Screenshots

### Home Page
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/93335399-b728-46b7-ae89-3fab8575ead0)


### Browse Event Page
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/0af734f5-850a-4ae9-8512-f7d30e69fe18)


### Private Event Creation Page
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/207670bb-04c2-4286-9d54-273847bd312e)


### User Dashboard
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/067f3b35-ba9a-4f29-9a1b-88231e544226)


### User Organized Events
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/c9b5e0c7-fca0-411f-8be7-de990ead7d85)


### User Purchased Events
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/a20536a2-dcef-4511-8c50-fe35d7a924c4)


### Admin Dashboard
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/cf4583ee-35f2-44de-8f63-c310566cb59f)

### User Events
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/2d596a62-e123-40bd-bccd-b0de2bd75cf6)

### Admin Events
![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/1fcb01b1-3073-4738-9a33-fe1809a75641)

