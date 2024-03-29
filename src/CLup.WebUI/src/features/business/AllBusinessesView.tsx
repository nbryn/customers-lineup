import React, {useState} from 'react';
import Chip from '@mui/material/Chip';
import {Col, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';
import {useHistory} from 'react-router';

import {useGetAllBusinessesQuery} from './BusinessApi';
import {setCurrentBusiness} from './BusinessState';
import {Header} from '../../shared/components/Texts';
import {MapModal, defaultMapProps} from '../../shared/components/modal/MapModal';
import type {MapModalProps} from '../../shared/components/modal/MapModal';
import type {TableColumn} from '../../shared/components/Table';
import {TableContainer} from '../../shared/containers/TableContainer';
import {useAppDispatch} from '../../app/Store';
import type {BusinessDto} from '../../autogenerated';

const useStyles = makeStyles(() => ({
    row: {
        justifyContent: 'center',
    },
}));

export const AllBusinessesView: React.FC = () => {
    const styles = useStyles();
    const history = useHistory();
    const dispatch = useAppDispatch();

    const {data: getAllBusinessResponse} = useGetAllBusinessesQuery();
    const [mapModalInfo, setMapModalInfo] = useState<MapModalProps>(defaultMapProps);

    const columns: TableColumn[] = [
        {title: 'id', field: 'id', hidden: true},
        {title: 'Name', field: 'name'},
        {title: 'City', field: 'city'},
        {title: 'Street', field: 'street'},
        {title: 'Business Hours', field: 'businessHours'},
        {title: 'Type', field: 'type'},
    ];

    const actions = [
        {
            icon: () => <Chip size="small" label="Go to business" clickable color="primary" />,
            tooltip: 'See available time slots',
            onClick: (_: any, business: BusinessDto) => {
                dispatch(setCurrentBusiness(business));
                history.push('/booking/new', {business});
            },
        },
        {
            icon: () => <Chip size="small" label="See on map" clickable color="secondary" />,
            tooltip: 'Show location on map',
            onClick: (_: any, business: BusinessDto) => {
                setMapModalInfo({
                    visible: true,
                    zoom: 14,
                    center: [
                        business.address?.coords?.longitude as number,
                        business.address?.coords?.latitude as number,
                    ],
                    markers: [
                        [
                            business.address?.coords?.longitude as number,
                            business.address?.coords?.longitude as number,
                        ],
                        13,
                    ],
                });
            },
        },
    ];

    return (
        <>
            <Row className={styles.row}>
                <Header text="Available Businesses" />
            </Row>
            <MapModal
                visible={mapModalInfo.visible}
                setVisible={() => setMapModalInfo(defaultMapProps)}
                zoom={mapModalInfo.zoom}
                center={mapModalInfo.center}
                markers={mapModalInfo.markers}
            />
            <Row className={styles.row}>
                <Col sm={6} md={8} xl={12}>
                    <TableContainer
                        actions={actions}
                        columns={columns}
                        tableData={getAllBusinessResponse?.businesses ?? []}
                        tableTitle="Businesses"
                    />
                </Col>
            </Row>
        </>
    );
};
