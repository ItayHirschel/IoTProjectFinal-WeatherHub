import logging
import os
import json
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty

# CONSTANTS
T_name = os.getenv("DT_table")
N = int(os.getenv("NumOfMeasurements"))


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    
    connection_string = os.getenv("AzureWebJobsStorage")

    param = req.params
    name = param["newUser"]
    friendly_name = param["FriendlyName"]
    account_name = param["Account"]

    if not name:
        return func.HttpResponse(
             "No Name",
             status_code=200
        )

    try:
        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            operations = []

            filter = f"PartitionKey eq '{name}'"
            entities = table.query_entities(filter)

            count = 0
            for ent in entities:
                count += 1

            if count > 0:
                return func.HttpResponse(f"Device {name} already registered", status_code=500)

            entity_partition = {"PartitionKey" : account_name, 
                                "RowKey" : name, 
                                "FriendlyName" : friendly_name,
                                "Location" : "null",
                                "TurnOffHour" : -1,
                                "TurnOnHour" : -1,
                                "TurnOffTemp" : 20,
                                "TurnOnTemp" : 30,
                                "AutoSaveHours" : False,
                                "AutoSaveTemp" : False,
                                "OnWebhook" : "",
                                "OffWebhook" : "",
                                "LastAssumedState" : False
                                }
            
            entity_pointer = {"PartitionKey" : name, "RowKey" : "pointer", "ptr" : 0}
            operations.append(("create", entity_pointer))

            for i in range(N):
                operations.append(("create", {"PartitionKey" : name, "RowKey" : str(i), "temperature": 0.0, "humidity" : 0.0, "pressure" : 0.0}))
            
            logging.info(operations)

            table.submit_transaction(operations)
            table.create_entity(entity_partition)

            return func.HttpResponse(f"User {name}, {friendly_name} of account {account_name} added", status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)

