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
import * as Steam_pb from './Steam_pb';


export class ModManagerServiceClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor(hostname: string,
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

  methodInfoListSteamGames = new grpcWeb.AbstractClientBase.MethodInfo(
    Steam_pb.ListSteamGamesResponse,
    (request: Steam_pb.ListSteamGamesRequest) => {
      return request.serializeBinary();
    },
    Steam_pb.ListSteamGamesResponse.deserializeBinary
  );

  listSteamGames(
    request: Steam_pb.ListSteamGamesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Steam_pb.ListSteamGamesResponse>;

  listSteamGames(
    request: Steam_pb.ListSteamGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
      response: Steam_pb.ListSteamGamesResponse) => void): grpcWeb.ClientReadableStream<Steam_pb.ListSteamGamesResponse>;

  listSteamGames(
    request: Steam_pb.ListSteamGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
      response: Steam_pb.ListSteamGamesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
        '/BepInEx.ModManagerService/ListSteamGames',
        request,
        metadata || {},
        this.methodInfoListSteamGames,
        callback);
    }
    return this.client_.unaryCall(
      this.hostname_ +
      '/BepInEx.ModManagerService/ListSteamGames',
      request,
      metadata || {},
      this.methodInfoListSteamGames);
  }

  methodInfoInstallBIE = new grpcWeb.AbstractClientBase.MethodInfo(
    Common_pb.CommonServiceResponse,
    (request: Steam_pb.InstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  installBIE(
    request: Steam_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Steam_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
      response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Steam_pb.InstallBIERequest,
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
    (request: Steam_pb.UninstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  uninstallBIE(
    request: Steam_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Steam_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
      response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Steam_pb.UninstallBIERequest,
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

}

