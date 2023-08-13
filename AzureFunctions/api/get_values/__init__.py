import logging
import os
import json
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty

# CONSTANTS
T_name = os.getenv("DT_table")
NUMBER_OF_READINGS = int(os.getenv("NumOfMeasurements"))

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    connection_string = os.getenv("AzureWebJobsStorage")

    param = req.params

    if param == None:

        param = {}
    if "User" in param.keys():

        PartitionKey = param["User"]



    try:
        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            
            filter = f"PartitionKey eq '{PartitionKey}'"


            entities = table.query_entities(filter)

            d = {}

            for ent in entities:
                d[ent["RowKey"]] = ent
            
            i_o = int(d["pointer"]["ptr"])

            lst = [None] * NUMBER_OF_READINGS

            for i in range(NUMBER_OF_READINGS):
                ind = (i_o + i) % NUMBER_OF_READINGS
                e = d[str(ind)]
                dico = {"TEMP" : e["temperature"], "PRES" : e["pressure"], "HUMID" : e["humidity"]}
                lst[i] = dico

            return func.HttpResponse(json.dumps(
                lst
            ), status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)

