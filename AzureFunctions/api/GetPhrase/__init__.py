import logging
import requests
import azure.functions as func
import os
import json

url = "https://atlas.microsoft.com/weather/currentConditions/json"

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    subscription_key = os.getenv("AzureMapsKey")
    try:
        coor = req.params.get('coordinates')
        par = {
        "api-version" : 1.0,
        "query" : coor,
        "subscription-key" : subscription_key
        }
        ret_message = requests.get(url, params=par).text

        dic = json.loads(ret_message)['results']

        phrase = dic[0]["phrase"]
        iconCode = dic[0]["iconCode"]
        humid = dic[0]["relativeHumidity"]
        logging.info("finished")
        return func.HttpResponse(f"{phrase},{iconCode},{humid}", status_code=200)
    except:
        return func.HttpResponse(f"something went wrong", status_code=500)
