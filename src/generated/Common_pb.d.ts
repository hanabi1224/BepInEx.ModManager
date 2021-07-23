import * as jspb from 'google-protobuf'



export class CommonServiceResponse extends jspb.Message {
  getSuccess(): boolean;
  setSuccess(value: boolean): CommonServiceResponse;

  getError(): string;
  setError(value: string): CommonServiceResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CommonServiceResponse.AsObject;
  static toObject(includeInstance: boolean, msg: CommonServiceResponse): CommonServiceResponse.AsObject;
  static serializeBinaryToWriter(message: CommonServiceResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CommonServiceResponse;
  static deserializeBinaryFromReader(message: CommonServiceResponse, reader: jspb.BinaryReader): CommonServiceResponse;
}

export namespace CommonServiceResponse {
  export type AsObject = {
    success: boolean,
    error: string,
  }
}

