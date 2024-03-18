import React from 'react';
import Chip from '@mui/material/Chip';
import {Col, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';

import {removeEmployee, selectEmployeesByBusiness} from './EmployeeState';
import type {EmployeeDTO} from './Employee';
import {ErrorView} from '../../../shared/views/ErrorView';
import {Header} from '../../../shared/components/Texts';
import {selectCurrentBusiness} from '../BusinessState';
import {useAppDispatch, useAppSelector} from '../../../app/Store';
import type {TableColumn} from '../../../shared/components/Table';
import {TableContainer} from '../../../shared/containers/TableContainer';

const useStyles = makeStyles(() => ({
    row: {
        justifyContent: 'center',
    },
}));

export const EmployeeView: React.FC = () => {
    const styles = useStyles();
    const dispatch = useAppDispatch();
    const business = useAppSelector(selectCurrentBusiness);

    if (!business) {
        return <ErrorView />;
    }

    const employees = useAppSelector(selectEmployeesByBusiness)

    const columns: TableColumn[] = [
        {title: 'BusinessId', field: 'businessId', hidden: true},
        {title: 'Name', field: 'name'},
        {title: 'Employed Since', field: 'employedSince'},
        {title: 'Private Email', field: 'privateEmail'},
        {title: 'Company Email', field: 'companyEmail'},
    ];

    const actions = [
        {
            icon: () => <Chip size="small" label="Remove Employee" clickable color="primary" />,
            onClick: async (event: any, employee: EmployeeDTO) => {
                dispatch(
                    removeEmployee({
                        id: employee.businessId!,
                        data: employee.privateEmail!,
                    })
                );
            },
        },
    ];

    return (
        <>
            <Row className={styles.row}>
                <Header text={business.name} />
            </Row>
            <Row className={styles.row}>
                <Col sm={6} md={8} lg={6} xl={10}>
                    <TableContainer
                        actions={actions}
                        columns={columns}
                        tableTitle="Employees"
                        tableData={employees}
                        emptyMessage="No Employees Yet"
                    />
                </Col>
            </Row>
        </>
    );
};
