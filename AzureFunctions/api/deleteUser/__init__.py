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
            entity_partition = {"PartitionKey" : account_name, "RowKey" : name}

            entity_pointer = {"PartitionKey" : name, "RowKey" : "pointer"}
            operations.append(("delete", entity_pointer))

            for i in range(N):
                operations.append(("delete", {"PartitionKey" : name, "RowKey" : str(i)}))
            
            logging.info(operations)

            table.submit_transaction(operations)
            table.delete_entity(entity_partition)

            return func.HttpResponse(f"Device {name} of Account {account_name} deleted", status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)

