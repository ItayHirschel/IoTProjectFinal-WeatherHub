import logging
import azure.functions as func


def main(req: func.HttpRequest, connectionInfo) -> func.HttpResponse:
    logging.info('Python HTTP trigger negoatioate function processed a request.')
    return func.HttpResponse(connectionInfo)
