# OrdersAPI

## Overview
This project implements an orders api to create orders and store in to a database (used SQLite for example in this project).
It leverages the EntityFramework.

![Api](readmeImg/image.png)

In Database:
![database](readmeImg/database.jpg)

## Api function demo
1. Insert a product with quantity 5.
   
![database](readmeImg/insertdata.jpg)

2. The api returns success with 201 and orderid.
   
![database](readmeImg/response.jpg)

3. The database contains the order just inserted.
 
![database](readmeImg/dbrecord.jpg)

4. If try to add an illegal order (repeated order id for example), will return corresponding error response.
   
![data repeated request](readmeImg/repeated.jpg)
![database error](readmeImg/dberror.jpg)

5. try add order with invalid request body (e.g. empty product list), will return corresponding error response.

![return error when empty items](readmeImg/emptyItems.jpg)
And db will not be affected when have error.
   


## Unit Test
This project consists 3 sets of test as of today. The startup test covers the program startup. The Controller tests the api controller and the utils tests the util methods.

Did not cover the migration part.

![database error](readmeImg/testcoverage.jpg)