"use client";

import { postData } from "@/utils/api";
import { Anchor, Button, Container, Group, Paper, PasswordInput, Text, TextInput } from "@mantine/core";
import { hasLength, isEmail, useForm } from "@mantine/form";
import { notifications } from "@mantine/notifications";
import Cookies from 'js-cookie';
import { useRouter } from "next/navigation";


export default function Login() {
    const router = useRouter();

    const form = useForm({
        mode: "controlled",
        initialValues: {email: "", password: ""},
        validate: {
            email: isEmail("Geçerli bir e-posta adresi girin"),
            password: hasLength({ min: 8 }, "En az 8 karakter olmalı"),
        },
    });

    const handleSubmit = async (values: typeof form.values) => {
        try{
            const res = await postData("api/Auth/Login", {email: values.email, password: values.password});
            if(res.data.status === "Success") {
                Cookies.set("accessToken", res.data.data.accessToken);
                Cookies.set("refreshToken", res.data.data.refreshToken, { expires: 15 });
                router.push("/")
            }
            else {
                notifications.show({
                    title: "Giriş Başarısız",
                    message: res.data.message,
                    color: "red",
                });
            }
        }
        catch (error) {
            console.error("Login failed:", error);
        }
    };

 
    return (
        <div>
            <form onSubmit={form.onSubmit(handleSubmit)}>
                <Container size={420} my={40}>
                    <Text className="text-center" size="32px" fw={700} mb={15}>
                        Tekrar hoş geldiniz
                    </Text>

                    <Text className="text-center" size="sm" c={"dimmed"}>
                        Henüz bir hesabınız yok mu? <Anchor href="/register"> Hesap oluştur </Anchor>
                    </Text>

                    <Paper withBorder shadow="sm" p={22} mt={30} radius="md">
                        <TextInput
                            {...form.getInputProps("email")}
                            label="Email"
                            placeholder="Email"
                            mt="md"
                            />
                        <PasswordInput 
                            {...form.getInputProps("password")}
                            label="Şifre" 
                            placeholder="Şifreniz" 
                            mt="md" 
                            />
                                                    
                        <Button fullWidth mt="xl" radius="md" type="submit" >
                            Giriş Yap
                        </Button>

                        <Group justify="center" mt="lg">
                            <Anchor href="/reset-password" size="sm">
                                Şifrenizi mi unuttunuz?
                            </Anchor>
                        </Group>
                    </Paper>
                </Container>
            </form>
        </div>
    );
}
