import logging
import os
import json
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty
from azure.eventgrid import EventGridPublisherClient, EventGridEvent
from azure.core.credentials import AzureKeyCredential

# CONSTANTS
PartitionKey = os.getenv("DT_partition")
T_name = os.getenv("DT_table")
NUMBER_OF_READINGS = int(os.getenv("NumOfMeasurements"))
TEMPERATURE_KEY = "TEMP"
HUMIDITY_KEY = "HUMID"
PRESSURE_KEY = "PRES"

def main(req: func.HttpRequest, signalRMessages: func.Out[str]) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    connection_string = os.getenv("AzureWebJobsStorage")
    PartitionKey = os.getenv("DT_partition")

    logging.info(req.params)

    json_req = req.params
    logging.info(str(json_req) + str(type(json_req)))

    if "User" in json_req.keys():
        PartitionKey = json_req["User"]
    
    temperature = float(json_req[TEMPERATURE_KEY])
    humidity = float(json_req[HUMIDITY_KEY])
    pressure = float(json_req[PRESSURE_KEY])

    try:
        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            entity_p = table.get_entity(PartitionKey, "pointer")
            logging.info('got pointer')
            i_0 = entity_p["ptr"]
            logging.info(f'pointer value {i_0}')
            i_1 = (i_0 + 1) % NUMBER_OF_READINGS
            entity_d = table.get_entity(PartitionKey, str(i_0))
            logging.info(f'got data ind {i_0}')
            entity_p["ptr"] = EntityProperty(i_1, "Edm.Int32")

            
            entity_d["temperature"] = temperature
            entity_d["humidity"] = humidity
            entity_d["pressure"] = pressure

            logging.info(f'changed entities')
            table.update_entity(entity_p)
            table.update_entity(entity_d)
            logging.info(f'updated entities')


            logging.info("send signalR")
            signalRMessages.set(json.dumps({
                'target': "SensorUpdate",
                'arguments' : [PartitionKey]
            }))
            logging.info("now querying")
            entities = table.query_entities(f"RowKey eq '{PartitionKey}'")
            
            ent = None
            temp = 0
            for enty in entities:
                temp += 1
                ent = enty
            logging.info(f"temp = {temp}")

            if (temp == 1) and (not (ent["TurnOffTemp"] <= temperature <= ent["TurnOnTemp"])):
                logging.info("len entities == 1")
                logging.info(ent)
                if ent["AutoSaveTemp"]:
                    logging.info("Query result AUTO ON : " +ent["PartitionKey"] + " , " + ent["RowKey"])
                    cred = AzureKeyCredential(os.getenv("EventGridKey"))
                    client = EventGridPublisherClient(os.getenv("EventGridHost"), cred)

                    client.send([
                        EventGridEvent(
                            event_type="Contoso.Item.ItemReceived",
                            data={
                                "device" : ent["RowKey"],
                                "account_key" : ent["PartitionKey"],
                                "temp" : temperature
                            },
                            subject="Temp",
                            data_version="2.0"
                        )
                    ])
                    client.close()
                else:
                    if (temperature >= ent["TurnOnTemp"]) and (ent["LastAssumedState"] == False):
                        signalRMessages.set(json.dumps({
                            'target': "InformWeatherWarm",
                            'arguments' : [ent["RowKey"]]
                        }))
                        ent["LastAssumedState"] = True
                        table.update_entity(ent)
                    elif (temperature <= ent["TurnOffTemp"]) and (ent["LastAssumedState"] == True):
                        signalRMessages.set(json.dumps({
                            'target': "InformWeatherCold",
                            'arguments' : [ent["RowKey"]]
                        }))
                        ent["LastAssumedState"] = False
                        table.update_entity(ent)

            return func.HttpResponse(f"{temperature},{humidity},{pressure} written in {i_0}, ptr is {i_1}", status_code=200)
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)
