import { ModManagerServiceClient } from "./generated/ServiceServiceClientPb";

// TODO: parse server port via url.
const serverPort = 40003;
export const grpcClient = new ModManagerServiceClient(`http://localhost:${serverPort}`, null, null);
