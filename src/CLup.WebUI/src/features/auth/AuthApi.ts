import Cookies from 'js-cookie';

import {AuthApi} from '../../autogenerated';
import type {LoginRequest, RegisterRequest} from '../../autogenerated';
import {USER_TAG, baseApi} from '../../app/Store';
import {apiMutation} from '../../shared/api/Api';

const LOGIN_FAILED_MSG = 'Wrong Email/Password';

const authApi = baseApi.injectEndpoints({
    endpoints: (builder) => ({
        login: builder.mutation<void, LoginRequest>({
            invalidatesTags: [USER_TAG],
            queryFn: async (loginRequest, api) => ({
                data: await apiMutation(
                    async (authApi) => {
                        const response = await authApi.login(loginRequest);
                        Cookies.set('access_token', response.data.token!);
                        return response;
                    },
                    AuthApi,
                    api,
                    {message: LOGIN_FAILED_MSG}
                ),
            }),
        }),
        register: builder.mutation<void, RegisterRequest>({
            invalidatesTags: [USER_TAG],
            queryFn: async (registerRequest, api) => ({
                data: await apiMutation(
                    async (authApi) => {
                        const response = await authApi.register(registerRequest);
                        Cookies.set('access_token', response.data.token!);
                        return response;
                    },
                    AuthApi,
                    api
                ),
            }),
        }),
    }),
});

export const {useLoginMutation, useRegisterMutation} = authApi;
