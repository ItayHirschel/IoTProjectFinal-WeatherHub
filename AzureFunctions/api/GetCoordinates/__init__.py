import logging
import requests
import azure.functions as func
import os
import json

url = "https://atlas.microsoft.com/search/address/json"

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    subscription_key = os.getenv("AzureMapsKey")
    try:
        address = req.params.get('query')
        par = {
        "api-version" : 1.0,
        "query" : address,
        "subscription-key" : subscription_key,
        "language" : "en-US"
        }
        ret_message = requests.get(url, params=par).text
        dic = json.loads(ret_message)['results']
        coors = str(dic[0]["position"]["lat"]) + "," + str(dic[0]["position"]["lon"])
        
        return func.HttpResponse(f"{coors}", status_code=200)
    except:
        return func.HttpResponse(f"something went wrong", status_code=500)
