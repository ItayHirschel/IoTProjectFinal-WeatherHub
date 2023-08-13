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
    Account_key = param["Account"]
    Device_name = param["Device"]

    arg_lst = ['AutoSaveHours', 'AutoSaveTemp', 'FriendlyName', 'Location', 'OffWebhook', 'OnWebhook', 'TurnOffHour', 'TurnOffTemp', 'TurnOnHour', 'TurnOnTemp']

    json_dictionary = req.get_json()

    if not all(key in json_dictionary.keys() for key in arg_lst):
        return func.HttpResponse( "Json File isn't of correct format", status_code=200)

    try:
        Account_key = param["Account"]
        Device_name = param["Device"]

        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')

            d = table.get_entity(Account_key, Device_name)

            for key in arg_lst:
                d[key] = json_dictionary[key]
                logging.info(f"ARGUMENT : {key} = {json_dictionary[key]}")
            logging.info(f"entity : {d}")
            table.update_entity(d)

            return func.HttpResponse("", status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)
    return func.HttpResponse( "", status_code=200)
