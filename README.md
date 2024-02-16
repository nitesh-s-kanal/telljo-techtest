**TellJO - Tech Test**

Thank you for the giving me this opportunity to take the test. It was very good and interesting. (Sorry for the bad formatting below)

**Pre-requesists:**
1. Open solution in Visual Studio and Update the connection string in appsettings.json to point to a Sql Server / Sql Server Express instance (no need to create a database)
3. In Package Manager Console, ensure the Data project is selected and run the command 'update-database'
4. Your database will be created/updated

**1. Bookmark management**
	
 Implement full CRUD management for Bookmarks. Users should be able to create a new category whilst creating a bookmark without requiring any page refresh.
-	Implemented bookmarks based on each category.
-	Implemented CRUD operations without page refresh (jQuery AJAX)
-	Categories are only visible when logged in.
-	Added UserId mapping to category table
  
**2. User accounts**
	
 The package has the default AspNetCore Identity installed however not implemented fully. Complete this implementation and change the entities to work on a per user basis. For additional credit, implement multiple membership providers allowing users to log in with OpenID services.
-	Implemented changes to work on per user basis for entities.
-	Validated User logins for bookmarks
 
**3. API access**
	Expose an API allowing external systems to manage bookmarks. You will need to consider authentication / access tokens.
-	Implemented access token-based Api calls to update bookmarks only.
 
**Below are the steps to get an access token **
-	Run the application locally.
-	Using Postman or any other tool, make a POST request to /api/login/oauth 
(eg: https://localhost:44326/api/login/oauth) add raw json data to the request with registered username and password.

**Request** - 
{
  "username":"admin@admin.com", 
  "password":"Admin@123"
}

**Response** 
{    
  "access_Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjdmY2VlOWUyLWUxODYtNDljYi04ZGRkLTc4NGVkZGU5YWFkMiIsIm5iZiI6MTcwODA5MDg0NCwiZXhwIjoxNzA4MDkxNzQ0LCJpYXQiOjE3MDgwOTA4NDQsImlzcyI6InRlc3QtaXNzdWVyIiwiYXVkIjoidGVzdC1hdWRpZW5jZSJ9.Kjr_6nD96I7YAEBfgN-f9Yb58uoKZERJ_j9KZDhZ4iI",
    "expiresIn": 15,
    "type": "Bearer"
}
 
Updating bookmarks externally.
1.	Add access token for Authorization (Bearer Token)
2.	Add additional header data [“clientid” : “registered email id”]

**GET (get all bookmarks for the user)**

**URL**: api/bookmarks/getallbookmarks

**Request**: {}

**GET (Specific bookmark)**

**URL**: api/bookmarks/getbookmark/{bookmark id}

**Request**: {}

**POST (Create bookmark)**

**URL**:  api/bookmarks/createbookmark

**Request**: 
{
    "url": "Admin Cat 1331",
    "shortDescription": "Admin Cat 1221",
    "categoryId": 9 
}


**PUT (Edit bookmark)**

**URL**: api/bookmarks/updatebookmark/{category id}

**Request**: 
{
    "id": 20,
    "url": "Admin Cat 1331",
    "shortDescription": "Admin Cat 1221",
    "categoryId": 9 
}

**DELETE (Delete bookmark)**

**URL**: api/bookmarks/deletebookmark/{bookmark id}

**Request**: {}

**Note:** Since API and MVC is running in a single application, the api access might always pass once its logged in, so please test api before testing the application flow. I did try to fix all the issue but there might be few unknown ones.

