import React, {useEffect, useState} from 'react';
import {Col, FormGroup, Row} from 'react-bootstrap';
import {makeStyles} from '@material-ui/core/styles';
import {useHistory, useLocation} from 'react-router-dom';

import {ApiCaller, useApi} from '../../hooks/useApi';
import {BusinessDTO} from '../../models/Business';
import {Card} from '../../components/card/Card';
import {ComboBox, ComboBoxOption} from '../../components/form/ComboBox';
import {employeeValidationSchema} from '../../validation/BusinessValidation';
import {ErrorView} from '../ErrorView';
import {Form} from '../../components/form/Form';
import {Modal} from '../../components/modal/Modal';
import {NewEmployeeDTO} from '../../models/Employee';
import {TextField} from '../../components/form/TextField';
import URL, {NEW_EMPLOYEE_URL} from '../../api/URL';
import {UserDTO} from '../../models/User';
import {useForm} from '../../hooks/useForm';

const useStyles = makeStyles((theme) => ({
    card: {
        marginTop: 60,
        borderRadius: 15,
        height: 600,
        textAlign: 'center',
    },
    formGroup: {
        marginBottom: 30,
    },
    helperText: {
        color: 'red',
    },
    textField: {
        width: '35%',
    },
    wrapper: {
        justifyContent: 'center',
    },
}));

interface LocationState {
    business: BusinessDTO;
}

const SUCCESS_MESSAGE = 'Employee Created - Go to my employees to see your employees';

export const NewEmployeeView: React.FC = () => {
    const styles = useStyles();
    const history = useHistory();
    const location = useLocation<LocationState>();

    const [users, setUsers] = useState<ComboBoxOption[]>([]);
    const [selectedUser, setSelectedUser] = useState<ComboBoxOption>({label: ''});
    const [showComboBox, setShowComBox] = useState<boolean>(true);

    const apiCaller: ApiCaller<UserDTO[]> = useApi(SUCCESS_MESSAGE);

    if (!location.state) {
        return <ErrorView />;
    }

    const {business} = location.state;

    const formValues: NewEmployeeDTO = {
        companyEmail: '',
    };

    const {formHandler, ...form} = useForm<NewEmployeeDTO>(
        formValues,
        employeeValidationSchema,
        NEW_EMPLOYEE_URL,
        'POST',
        apiCaller.mutation
    );

    useEffect(() => {
        (async () => {
            const users = await apiCaller.query(
                URL.getAllUsersNotAlreadyEmployedByBusinessURL(business.id)
            );

            setUsers(
                users.map((user) => ({
                    label: user.email,
                    value: user.name,
                }))
            );
        })();
    }, []);

    useEffect(() => {
        if (users.find((user) => user.label === selectedUser?.label)) {
            setShowComBox(false);
        }
    }, [selectedUser]);

    return (
        <Row className={styles.wrapper}>
            <Col sm={6} lg={6}>
                <Modal
                    show={apiCaller.requestInfo ? true : false}
                    title="Employee Info"
                    text={apiCaller.requestInfo}
                    primaryAction={() => history.push('/business/employees/manage')}
                    primaryActionText="My Employees"
                    secondaryAction={() => apiCaller.setRequestInfo('')}
                />
                <Card
                    className={styles.card}
                    title="New Employee"
                    subtitle="Choose an existing user you want to add as an employee"
                    variant="outlined"
                >
                    {showComboBox && (
                        <ComboBox
                            style={{marginTop: 10, marginLeft: 110, width: '60%'}}
                            label="Email"
                            id="email"
                            type="text"
                            options={users}
                            setFieldValue={(option: ComboBoxOption) => setSelectedUser(option)}
                            partOfForm={false}
                        />
                    )}
                    {!showComboBox && (
                        <>
                            <Form
                                style={{marginTop: 20}}
                                onSubmit={(e: React.FormEvent) => {
                                    e.preventDefault();
                                    form.setRequest({
                                        businessId: business.id,
                                        privateEmail: selectedUser?.label,
                                        companyEmail: formHandler.values.companyEmail,
                                    });

                                    formHandler.handleSubmit();
                                }}
                                buttonText="Create"
                                working={apiCaller.working}
                                valid={formHandler.isValid}
                            >
                                <FormGroup className={styles.formGroup}>
                                    <TextField
                                        className={styles.textField}
                                        id="name"
                                        label="Name"
                                        type="text"
                                        value={selectedUser.value}
                                        disabled={true}
                                    />
                                </FormGroup>
                                <FormGroup className={styles.formGroup}>
                                    <TextField
                                        className={styles.textField}
                                        id="privateEmail"
                                        label="Private Email"
                                        type="email"
                                        value={selectedUser.label}
                                        disabled={true}
                                    />
                                </FormGroup>
                                <FormGroup className={styles.formGroup}>
                                    <TextField
                                        className={styles.textField}
                                        id="companyEmail"
                                        label="Company Email"
                                        type="email"
                                        value={formHandler.values.companyEmail}
                                        onChange={formHandler.handleChange}
                                        onBlur={formHandler.handleBlur}
                                        error={
                                            formHandler.touched.companyEmail &&
                                            Boolean(formHandler.errors.companyEmail)
                                        }
                                        helperText={
                                            formHandler.touched.companyEmail &&
                                            formHandler.errors.companyEmail
                                        }
                                    />
                                </FormGroup>
                            </Form>
                        </>
                    )}
                </Card>
            </Col>
        </Row>
    );
};
