import logging
import os
import json
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    connection_string = os.getenv("AzureWebJobsStorage")
    T_name = os.getenv("DT_table")

    param = req.params

    try:
        Account_key = param["Account"]
        Device_name = param["Device"]

        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            
            entity_p = table.get_entity(Account_key, Device_name)

            d = {}

            for key in entity_p.keys():
                if "PartitionKey" != key:
                    d[key] = entity_p[key]

            return func.HttpResponse(json.dumps(
                d
            ), status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)
