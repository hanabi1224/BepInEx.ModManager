/**
 * @fileoverview gRPC-Web generated client stub for BepInEx
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Common_pb from './Common_pb';
import * as Game_pb from './Game_pb';
import * as Service_pb from './Service_pb';
import * as Repo_pb from './Repo_pb';


export class ModManagerServiceClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'text';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodInfoListGames = new grpcWeb.AbstractClientBase.MethodInfo(
    Game_pb.ListGamesResponse,
    (request: Game_pb.ListGamesRequest) => {
      return request.serializeBinary();
    },
    Game_pb.ListGamesResponse.deserializeBinary
  );

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Game_pb.ListGamesResponse>;

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Game_pb.ListGamesResponse) => void): grpcWeb.ClientReadableStream<Game_pb.ListGamesResponse>;

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Game_pb.ListGamesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/ListGames',
        request,
        metadata || {},
        this.methodInfoListGames,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/ListGames',
    request,
    metadata || {},
    this.methodInfoListGames);
  }

  methodInfoInstallBIE = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Game_pb.InstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/InstallBIE',
        request,
        metadata || {},
        this.methodInfoInstallBIE,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/InstallBIE',
    request,
    metadata || {},
    this.methodInfoInstallBIE);
  }

  methodInfoUninstallBIE = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Game_pb.UninstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/UninstallBIE',
        request,
        metadata || {},
        this.methodInfoUninstallBIE,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/UninstallBIE',
    request,
    metadata || {},
    this.methodInfoUninstallBIE);
  }

  methodInfoCheckPluginUpdates = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.CheckPluginUpdatesRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/CheckPluginUpdates',
        request,
        metadata || {},
        this.methodInfoCheckPluginUpdates,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/CheckPluginUpdates',
    request,
    metadata || {},
    this.methodInfoCheckPluginUpdates);
  }

  methodInfoListPlugins = new grpcWeb.AbstractClientBase.MethodInfo(
    Repo_pb.ListPluginsResponse,
    (request: Repo_pb.ListPluginsRequest) => {
      return request.serializeBinary();
    },
    Repo_pb.ListPluginsResponse.deserializeBinary
  );

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null): Promise<Repo_pb.ListPluginsResponse>;

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Repo_pb.ListPluginsResponse) => void): grpcWeb.ClientReadableStream<Repo_pb.ListPluginsResponse>;

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Repo_pb.ListPluginsResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/ListPlugins',
        request,
        metadata || {},
        this.methodInfoListPlugins,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/ListPlugins',
    request,
    metadata || {},
    this.methodInfoListPlugins);
  }

  methodInfoAddPluginToRepo = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.AddPluginToRepoRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/AddPluginToRepo',
        request,
        metadata || {},
        this.methodInfoAddPluginToRepo,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/AddPluginToRepo',
    request,
    metadata || {},
    this.methodInfoAddPluginToRepo);
  }

  methodInfoInstallPlugin = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.InstallPluginRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/InstallPlugin',
        request,
        metadata || {},
        this.methodInfoInstallPlugin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/InstallPlugin',
    request,
    metadata || {},
    this.methodInfoInstallPlugin);
  }

  methodInfoUninstallPlugin = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.UninstallPluginRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/UninstallPlugin',
        request,
        metadata || {},
        this.methodInfoUninstallPlugin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/UninstallPlugin',
    request,
    metadata || {},
    this.methodInfoUninstallPlugin);
  }

  methodInfoLongConnect = new grpcWeb.AbstractClientBase.MethodInfo(
    Service_pb.LongConnectResponse,
    (request: Service_pb.LongConnectRequest) => {
      return request.serializeBinary();
    },
    Service_pb.LongConnectResponse.deserializeBinary
  );

  longConnect(
    request: Service_pb.LongConnectRequest,
    metadata?: grpcWeb.Metadata) {
    return this.client_.serverStreaming(
      this.hostname_ +
        '/BepInEx.ModManagerService/LongConnect',
      request,
      metadata || {},
      this.methodInfoLongConnect);
  }

}

