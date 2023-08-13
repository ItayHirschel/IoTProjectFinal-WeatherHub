import logging
import os
import json
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty

# CONSTANTS
PartitionKey = "Partitions"
T_name = os.getenv("DT_table")


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    connection_string = os.getenv("AzureWebJobsStorage")
    param = req.params
    try:
        PartitionKey = param["Account"]
        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            
            filter = f"PartitionKey eq '{PartitionKey}'"

            #print(filter)

            entities = table.query_entities(filter)

            #print("after filter")

            lst = []

            for ent in entities:
                dictionary = {}
                dictionary["Serial"] = ent["RowKey"]
                dictionary["Name"] = ent["FriendlyName"]
                lst.append(dictionary)

            #logging.info("after loop")

            return func.HttpResponse(json.dumps(
                lst
            ), status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)

