import axios, {type AxiosError, type AxiosResponse} from 'axios';
import Cookies from 'js-cookie';
import {type SerializedError} from '@reduxjs/toolkit';
import {type BaseQueryApi} from '@reduxjs/toolkit/query';

import {Configuration, QueryApi, type TokenResponse} from '../../autogenerated';
import {type ApiInfo, setApiState} from './ApiState';

export async function apiQuery<TResponse>(
    query: (queryApi: QueryApi) => Promise<AxiosResponse<TResponse, any>>,
    api: BaseQueryApi,
    successInfo?: Omit<ApiInfo, 'error'>
): Promise<TResponse> {
    try {
        const response = await query(new QueryApi(getConfiguration()));
        if (successInfo) {
            api.dispatch(setApiState(successInfo));
        }

        return response.data;
    } catch (error) {
        return handleError(api, error);
    }
}

export async function apiMutation<TApi>(
    mutation: (api: TApi) => Promise<AxiosResponse<void | TokenResponse, any>>,
    apiConstructor: new (config: Configuration) => TApi,
    api: BaseQueryApi,
    successInfo?: Omit<ApiInfo, 'error'>
): Promise<void> {
    try {
        await mutation(new apiConstructor(getConfiguration()));
        if (successInfo) {
            api.dispatch(setApiState(successInfo));
        }
    } catch (error) {
        handleError(api, error);
    }
}

function getConfiguration() {
    return new Configuration({
        basePath: process.env.REACT_APP_API_URL,
        accessToken: Cookies.get('access_token'),
    });
}

function handleError(api: BaseQueryApi, error: any) {
    console.warn('API call failed!');
    console.error(error);
    if (axios.isAxiosError(error)) {
        const axiosError = error as AxiosError;
        api.dispatch(setApiState({message: axiosError.response?.data, error: true}));

        return undefined as any;
    }

    const err = error as SerializedError;
    api.dispatch(setApiState({message: err.message, error: true}));

    return undefined as any;
}
