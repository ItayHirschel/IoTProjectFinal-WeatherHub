import json
import logging
from azure.data.tables import TableClient, EntityProperty
import azure.functions as func
from datetime import datetime
import os
import requests
from azure.iot.hub import IoTHubRegistryManager

URL = "https://atlas.microsoft.com/timezone/byCoordinates/json"

def main(event: func.EventGridEvent):
    if event.subject == "Hour":
        logging.info("CASE : HOURLY")
        case_subject_hour(event)
    elif event.subject == "Temp":
        logging.info("CASE : TEMPERATURE")
        case_subject_temp(event)
    else:
        logging.info(f"subject {event.subject} not recognized")

    


def case_subject_hour(event: func.EventGridEvent):
    data = event.get_json()
    logging.info(data)
    

    dev_name = data['device']
    acc_key = data['account_key']
    T_name=os.getenv("DT_table")
    table_connection_string = os.getenv("AzureWebJobsStorage")
    
    logging.info("CP 1")
    try:
        with TableClient.from_connection_string(table_connection_string, table_name=T_name) as table:
            logging.info('Table opened')

            entity_p = table.get_entity(acc_key, dev_name)
            query = entity_p["Location"].replace(";",",")

            logging.info(f"query is {query}")

            par = {
                "api-version" : 1.0,
                "query" : query,
                "subscription-key" : os.getenv("AzureMapsKey")
            }
            ret_message = json.loads(requests.get(URL, params=par).text)
            logging.info("CP 2")
            currtime = datetime.strptime(ret_message["TimeZones"][0]["ReferenceTime"]["WallTime"].split('.')[0], "%Y-%m-%dT%H:%M:%S")
            logging.info("CP 3")
            hour = currtime.hour
            logging.info("CP 4")

            on_hour = entity_p["TurnOnHour"]
            off_hour = entity_p["TurnOffHour"]
            logging.info(f"currHour : {hour}, onHour : {on_hour}, offHour : {off_hour}")

            if hour == on_hour:

                connection_string = os.getenv("IoTHubConnectionString")
                registry_manager = IoTHubRegistryManager(connection_string)
                registry_manager.send_c2d_message(str(entity_p["RowKey"]), str(entity_p["OnWebhook"]))
                logging.info(f"sent c2d, device : " + entity_p["RowKey"] + ", msg : " + entity_p["OnWebhook"] )
                entity_p["LastAssumedState"] = True
                table.update_entity(entity_p)
            
            if hour == off_hour:

                connection_string = os.getenv("IoTHubConnectionString")
                registry_manager = IoTHubRegistryManager(connection_string)
                registry_manager.send_c2d_message(str(entity_p["RowKey"]), str(entity_p["OffWebhook"]))
                logging.info(f"sent c2d, device : " + entity_p["RowKey"] + ", msg : " + entity_p["OffWebhook"] )
                entity_p["LastAssumedState"] = False
                table.update_entity(entity_p)

    except:
        logging.info('Python EventGrid trigger function failed')

def case_subject_temp(event: func.EventGridEvent):
    data = event.get_json()
    logging.info(data)
    

    dev_name = data['device']
    acc_key = data['account_key']
    curr_temp = data['temp']
    T_name=os.getenv("DT_table")
    table_connection_string = os.getenv("AzureWebJobsStorage")
    
    logging.info("CP 1")
    try:
        with TableClient.from_connection_string(table_connection_string, table_name=T_name) as table:
            logging.info('Table opened')

            entity_p = table.get_entity(acc_key, dev_name)

            on_temp = entity_p["TurnOnTemp"]
            off_temp = entity_p["TurnOffTemp"]
            logging.info(f"currTemp : {curr_temp}, onTemp : {on_temp}, offTemp : {off_temp}")

            if curr_temp >= on_temp:

                connection_string = os.getenv("IoTHubConnectionString")
                registry_manager = IoTHubRegistryManager(connection_string)
                registry_manager.send_c2d_message(str(entity_p["RowKey"]), str(entity_p["OnWebhook"]))
                logging.info(f"sent c2d, device : " + entity_p["RowKey"] + ", msg : " + entity_p["OnWebhook"] )
                entity_p["LastAssumedState"] = True
                table.update_entity(entity_p)
            
            if curr_temp <= off_temp:

                connection_string = os.getenv("IoTHubConnectionString")
                registry_manager = IoTHubRegistryManager(connection_string)
                registry_manager.send_c2d_message(str(entity_p["RowKey"]), str(entity_p["OffWebhook"]))
                logging.info(f"sent c2d, device : " + entity_p["RowKey"] + ", msg : " + entity_p["OffWebhook"] )
                entity_p["LastAssumedState"] = False
                table.update_entity(entity_p)

    except:
        logging.info('Python EventGrid trigger function failed')

    
