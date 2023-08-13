import logging
from azure.eventgrid import EventGridPublisherClient, EventGridEvent
from azure.core.credentials import AzureKeyCredential
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty
import os

topic_key = os.getenv("EventGridKey")
endpoint = os.getenv("EventGridHost")

def main(mytimer: func.TimerRequest) -> None:
    logging.info('Python HTTP trigger function processed a request.')
    try:
        cred = AzureKeyCredential(topic_key)
        client = EventGridPublisherClient(endpoint, cred)
        connection_string = os.getenv("AzureWebJobsStorage")
        T_name = os.getenv("DT_table")
        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')
            
            filter = f"AutoSaveHours eq true"

            entities = table.query_entities(filter)
            logging.info('CP 1')
            lst =[]

            for ent in entities:
                lst.append(
                    EventGridEvent(
                        event_type="Contoso.Item.ItemReceived",
                        data={
                            "device" : ent["RowKey"],
                            "account_key" : ent["PartitionKey"]
                        },
                        subject="Hour",
                        data_version="2.0"
                    )
                )
                logging.info("device : " + ent["RowKey"] + ", account_key : " + ent["PartitionKey"])
                
            if len(lst) > 0:
                client.send(lst)

            logging.info('CP 2')
            client.close()
            logging.info('Finished successfully')
            logging.info(f"Allright then")
    except:
        logging.info(f"Allwrong then")
