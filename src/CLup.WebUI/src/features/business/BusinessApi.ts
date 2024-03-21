import Cookies from 'js-cookie';

import {BUSINESS_BY_OWNER_TAG, BUSINESS_TAG, emptySplitApi, USER_TAG} from '../../app/Store';
import {
    BusinessApi,
    Configuration,
    QueryApi,
    type CreateBusinessRequest,
    type GetAllBusinessesResponse,
    type UpdateBusinessRequest,
} from '../../autogenerated';

const queryApi = new QueryApi(new Configuration({accessToken: Cookies.get('access_token')}));
const businessApiInstance = new BusinessApi(
    new Configuration({accessToken: Cookies.get('access_token')})
);

const businessApi = emptySplitApi.injectEndpoints({
    endpoints: (builder) => ({
        getAllBusinesses: builder.query<GetAllBusinessesResponse, void>({
            providesTags: [BUSINESS_TAG, BUSINESS_BY_OWNER_TAG, USER_TAG],
            queryFn: async () => {
                try {
                    const response = await queryApi.getAllBusinesses();
                    return {data: response.data};
                } catch (error) {
                    return {error} as {data: GetAllBusinessesResponse; error: any};
                }
            },
        }),
        updateBusiness: builder.mutation<void, UpdateBusinessRequest>({
            queryFn: async (updateBusinessRequest) => {
                try {
                    await businessApiInstance.updateBusiness(updateBusinessRequest);
                } catch (error) {
                    return {error} as any;
                }
            },
        }),
        createBusiness: builder.mutation<void, CreateBusinessRequest>({
            invalidatesTags: [BUSINESS_BY_OWNER_TAG, USER_TAG],
            queryFn: async (createBusinessRequest) => {
                try {
                    await businessApiInstance.createBusiness(createBusinessRequest);
                } catch (error) {
                    return {error} as any;
                }
            },
        }),
    }),
});

export const {useGetAllBusinessesQuery, useUpdateBusinessMutation, useCreateBusinessMutation} = businessApi;