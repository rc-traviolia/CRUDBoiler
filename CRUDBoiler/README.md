# CRUDBoiler
This package makes writing basic CRUD functionality into Azure Function Apps (isolated) easier. By making your function extend the CRUDFunction class, and then appropriately implementing the IngestionPoint method, you will be all set to go- just implement the CRUD methods as you desire.

Alternatively, you can use the CRUDFunctionWithService to impelment your CRUD logic in a separate service. Just make sure it implements the ICRUDService with the appropriate TDataModel and TResponse model and add it to your services and it will be ready for dependency injection.