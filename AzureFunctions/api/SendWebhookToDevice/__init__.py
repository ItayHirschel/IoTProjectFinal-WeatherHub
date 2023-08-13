import logging

import azure.functions as func
import os
from azure.iot.hub import IoTHubRegistryManager

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    connection_string = os.getenv("IoTHubConnectionString")
    device_id = req.params.get('device')
    webhook = req.params.get('webhook')

    try:
        registry_manager = IoTHubRegistryManager(connection_string)
        registry_manager.send_c2d_message(str(device_id), str(webhook))
        return func.HttpResponse("", status_code=200)
    except:
        return func.HttpResponse("something went wrong", status_code=500)
