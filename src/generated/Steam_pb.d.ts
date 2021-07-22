import * as jspb from 'google-protobuf'



export class ListSteamGamesRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListSteamGamesRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListSteamGamesRequest): ListSteamGamesRequest.AsObject;
  static serializeBinaryToWriter(message: ListSteamGamesRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListSteamGamesRequest;
  static deserializeBinaryFromReader(message: ListSteamGamesRequest, reader: jspb.BinaryReader): ListSteamGamesRequest;
}

export namespace ListSteamGamesRequest {
  export type AsObject = {
  }
}

export class ListSteamGamesResponse extends jspb.Message {
  getGamesList(): Array<GameInfo>;
  setGamesList(value: Array<GameInfo>): ListSteamGamesResponse;
  clearGamesList(): ListSteamGamesResponse;
  addGames(value?: GameInfo, index?: number): GameInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListSteamGamesResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListSteamGamesResponse): ListSteamGamesResponse.AsObject;
  static serializeBinaryToWriter(message: ListSteamGamesResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListSteamGamesResponse;
  static deserializeBinaryFromReader(message: ListSteamGamesResponse, reader: jspb.BinaryReader): ListSteamGamesResponse;
}

export namespace ListSteamGamesResponse {
  export type AsObject = {
    gamesList: Array<GameInfo.AsObject>,
  }
}

export class GameInfo extends jspb.Message {
  getId(): string;
  setId(value: string): GameInfo;

  getName(): string;
  setName(value: string): GameInfo;

  getPath(): string;
  setPath(value: string): GameInfo;

  getIsbieinstalled(): boolean;
  setIsbieinstalled(value: boolean): GameInfo;

  getIsbieinitialized(): boolean;
  setIsbieinitialized(value: boolean): GameInfo;

  getPluginsList(): Array<PluginInfo>;
  setPluginsList(value: Array<PluginInfo>): GameInfo;
  clearPluginsList(): GameInfo;
  addPlugins(value?: PluginInfo, index?: number): PluginInfo;

  getPatchersList(): Array<PatcherInfo>;
  setPatchersList(value: Array<PatcherInfo>): GameInfo;
  clearPatchersList(): GameInfo;
  addPatchers(value?: PatcherInfo, index?: number): PatcherInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GameInfo.AsObject;
  static toObject(includeInstance: boolean, msg: GameInfo): GameInfo.AsObject;
  static serializeBinaryToWriter(message: GameInfo, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GameInfo;
  static deserializeBinaryFromReader(message: GameInfo, reader: jspb.BinaryReader): GameInfo;
}

export namespace GameInfo {
  export type AsObject = {
    id: string,
    name: string,
    path: string,
    isbieinstalled: boolean,
    isbieinitialized: boolean,
    pluginsList: Array<PluginInfo.AsObject>,
    patchersList: Array<PatcherInfo.AsObject>,
  }
}

export class PluginInfo extends jspb.Message {
  getId(): string;
  setId(value: string): PluginInfo;

  getName(): string;
  setName(value: string): PluginInfo;

  getVersion(): string;
  setVersion(value: string): PluginInfo;

  getPath(): string;
  setPath(value: string): PluginInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PluginInfo.AsObject;
  static toObject(includeInstance: boolean, msg: PluginInfo): PluginInfo.AsObject;
  static serializeBinaryToWriter(message: PluginInfo, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PluginInfo;
  static deserializeBinaryFromReader(message: PluginInfo, reader: jspb.BinaryReader): PluginInfo;
}

export namespace PluginInfo {
  export type AsObject = {
    id: string,
    name: string,
    version: string,
    path: string,
  }
}

export class PatcherInfo extends jspb.Message {
  getId(): string;
  setId(value: string): PatcherInfo;

  getName(): string;
  setName(value: string): PatcherInfo;

  getVersion(): string;
  setVersion(value: string): PatcherInfo;

  getPath(): string;
  setPath(value: string): PatcherInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PatcherInfo.AsObject;
  static toObject(includeInstance: boolean, msg: PatcherInfo): PatcherInfo.AsObject;
  static serializeBinaryToWriter(message: PatcherInfo, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PatcherInfo;
  static deserializeBinaryFromReader(message: PatcherInfo, reader: jspb.BinaryReader): PatcherInfo;
}

export namespace PatcherInfo {
  export type AsObject = {
    id: string,
    name: string,
    version: string,
    path: string,
  }
}

