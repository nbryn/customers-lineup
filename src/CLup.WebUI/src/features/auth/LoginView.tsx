import {Col, Container, FormGroup, Row} from 'react-bootstrap';
import makeStyles from '@mui/styles/makeStyles';
import React, {useState} from 'react';

import {Card} from '../../shared/components/card/Card';
import {Form} from '../../shared/components/form/Form';
import {loginValidationSchema} from '../user/UserValidation';
import {RegisterView} from './RegisterView';
import StringUtil from '../../shared/util/StringUtil';
import {TextField} from '../../shared/components/form/TextField';
import TextFieldUtil from '../../shared/util/TextFieldUtil';
import {useForm} from '../../shared/hooks/useForm';
import type {Index} from '../../shared/hooks/useForm';
import {useLoginMutation} from './AuthApi';

const useStyles = makeStyles(() => ({
    button: {
        width: '38%',
    },
    card: {
        marginTop: 60,
        borderRadius: 15,
        height: 450,
        //boxShadow: '0px 0px 0px 8px rgba(12, 12, 242, 0.1)',
        textAlign: 'center',
    },
    textField: {
        width: '42%',
    },
    wrapper: {
        justifyContent: 'center',
    },
}));

export const LoginView: React.FC = () => {
    const styles = useStyles();

    const [renderRegisterView, setRenderRegisterView] = useState(false);
    const [login] = useLoginMutation();

    const formValues = {
        email: '',
        password: '',
    };

    const {formHandler} = useForm<typeof formValues & Index>({
        initialValues: formValues,
        validationSchema: loginValidationSchema,
        onSubmit: async (data) => await login({...data}),
    });

    return (
        <Container>
            {renderRegisterView ? (
                <RegisterView />
            ) : (
                <Row className={styles.wrapper}>
                    <Col sm={10} lg={6}>
                        <Card
                            className={styles.card}
                            title="Login"
                            buttonAction={() => setRenderRegisterView(true)}
                            buttonColor="secondary"
                            buttonText="Register"
                            buttonSize="medium"
                            buttonStyle={styles.button}
                            variant="outlined"
                        >
                            <Form
                                onSubmit={formHandler.handleSubmit}
                                buttonText="Login"
                                valid={formHandler.isValid}
                            >
                                {Object.keys(formValues).map((key) => (
                                    <FormGroup key={key}>
                                        <TextField
                                            className={styles.textField}
                                            id={key}
                                            label={StringUtil.capitalize(key)}
                                            type={TextFieldUtil.mapKeyToType(key)}
                                            value={formHandler.values[key] as string}
                                            onChange={formHandler.handleChange}
                                            onBlur={formHandler.handleBlur}
                                            error={
                                                formHandler.touched[key] &&
                                                !!formHandler.errors[key]
                                            }
                                            helperText={
                                                formHandler.touched[key] && formHandler.errors[key]
                                            }
                                            variant="outlined"
                                        />
                                    </FormGroup>
                                ))}
                            </Form>
                        </Card>
                    </Col>
                </Row>
            )}
        </Container>
    );
};
