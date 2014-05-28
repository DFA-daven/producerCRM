namespace CallForm.Core
{

    class WebBackEnd
    {
        // todo: get these from web.config
        // name fields
        public string webTestServiceProtocol = "http://";
        public string webTestServiceIPName = "dl-backend-02.azurewebsites.net";
        public string webProductionServiceProtocol = "http://";
        public string webProductionServiceIPName = "dl-backend.azurewebsites.net";

        // properties
        public string TestTarget
        {
            get { return webTestServiceProtocol + webTestServiceIPName; }
        }

        public string ProductionTarget
        {
            get { return webProductionServiceProtocol + webProductionServiceIPName; }
        }
    }
}
