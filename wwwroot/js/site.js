(() => {
	const tokenKey = "parkg.jwt";
	const loginPath = "/login";
	const registerPath = "/register";
	const dashboardPath = "/dashboard";
	window.ParkGAuth = {
		getValidToken: () => getValidToken(),
		isTokenValid: (token) => isTokenValid(token),
		tokenKey
	};
	const publicPaths = new Set([loginPath, registerPath, "/error"]);

	const getCurrentPath = () => window.location.pathname.toLowerCase();

	const base64UrlDecode = (value) => {
		const base64 = value.replace(/-/g, "+").replace(/_/g, "/");
		const padded = base64 + "=".repeat((4 - (base64.length % 4)) % 4);
		return decodeURIComponent(
			atob(padded)
				.split("")
					.map((character) => `%${(`00${character.charCodeAt(0).toString(16)}`).slice(-2)}`)
					.join("")
		);
	};

	const getTokenPayload = (token) => {
		try {
			const payloadPart = token.split(".")[1];
			if (!payloadPart) {
				return null;
			}

			return JSON.parse(base64UrlDecode(payloadPart));
		} catch {
			return null;
		}
	};

	const isTokenValid = (token) => {
		if (!token) {
			return false;
		}

		const payload = getTokenPayload(token);
		if (!payload?.exp) {
			return false;
		}

		return Date.now() < (payload.exp * 1000);
	};

	const getValidToken = () => {
		const token = localStorage.getItem(tokenKey);
		if (!isTokenValid(token)) {
			localStorage.removeItem(tokenKey);
			return null;
		}

		return token;
	};

	const redirectTo = (path) => {
		window.location.replace(path);
	};

	const enforceRouteGuard = () => {
		const currentPath = getCurrentPath();
		const token = getValidToken();

		if ((currentPath === loginPath || currentPath === registerPath) && token) {
			redirectTo(dashboardPath);
			return true;
		}

		if (!publicPaths.has(currentPath) && !token) {
			redirectTo(loginPath);
			return true;
		}

		return false;
	};

	const setTheme = (theme) => {
		document.body.setAttribute("data-theme", theme);
		localStorage.setItem("parkg.theme", theme);
	};

	const syncTheme = () => {
		const savedTheme = localStorage.getItem("parkg.theme") || "light";
		setTheme(savedTheme);
	};

	const stringify = (value) => {
		if (typeof value === "string") {
			return value;
		}

		if (value && typeof value === "object") {
			if (typeof value.message === "string") {
				return value.message;
			}

			return Object.entries(value)
				.map(([key, entryValue]) => `${key}: ${Array.isArray(entryValue) ? entryValue.join(", ") : entryValue}`)
				.join("\n");
		}

		return String(value);
	};

	const currencyFormatter = new Intl.NumberFormat("es-PE", {
		style: "currency",
		currency: "PEN",
		minimumFractionDigits: 2,
		maximumFractionDigits: 2
	});

	const formatCurrency = (value) => currencyFormatter.format(Number(value ?? 0));

	const formatDecimal = (value) => Number(value ?? 0).toFixed(2);

	const setTextContent = (selector, value) => {
		const element = document.querySelector(selector);
		if (!element) {
			return;
		}

		element.textContent = value;
	};

	const setStatusChip = (selector, value) => {
		const element = document.querySelector(selector);
		if (!element) {
			return;
		}

		element.textContent = value;
	};

	const setTariffSummary = (tarifas) => {
		if (!Array.isArray(tarifas)) {
			return;
		}

		tarifas.forEach((tarifa) => {
			const vehicle = tarifa.tipoVehiculo?.toLowerCase?.() ?? tarifa.tipoVehiculo;
			setTextContent(`[data-currency-field="${vehicle}-hora"]`, formatCurrency(tarifa.tarifaHora));
			setTextContent(`[data-currency-note="${vehicle}-dia"]`, `Día: ${formatCurrency(tarifa.tarifaDia)}`);
		});
	};

	const renderHistoryPlaceholder = () => {
		const history = document.getElementById("tarifasHistory");
		if (!history) {
			return;
		}

		history.innerHTML = `
			<div class="history-card">
				<strong>Sin versiones históricas</strong>
				<small>Este espacio quedará conectado al historial de cambios cuando se habilite el backend correspondiente.</small>
			</div>`;
	};

	const populateSpaceOptions = (spaces, selectedCode = "") => {
		const select = document.getElementById("EspacioCodigoIngreso");
		if (!select) {
			return;
		}

		select.innerHTML = "";

		if (!spaces || spaces.length === 0) {
			const option = document.createElement("option");
			option.value = "";
			option.textContent = "No hay espacios libres compatibles";
			select.append(option);
			select.disabled = true;
			return;
		}

		select.disabled = false;
		const placeholder = document.createElement("option");
		placeholder.value = "";
		placeholder.textContent = "Selecciona un espacio";
		select.append(placeholder);

		spaces.forEach((space) => {
			const option = document.createElement("option");
			option.value = space.codigo;
			option.textContent = `${space.codigo} · ${space.tipoVehiculoPermitido}`;
			if (space.codigo === selectedCode) {
				option.selected = true;
			}
			select.append(option);
		});
	};

	const setInputError = (input, errorElement, message) => {
		if (!input || !errorElement) {
			return;
		}

		input.classList.toggle("is-invalid", Boolean(message));
		input.setAttribute("aria-invalid", String(Boolean(message)));
		errorElement.textContent = message || "";
		errorElement.hidden = !message;
	};

	const clearIngressErrors = () => {
		setInputError(document.getElementById("PlacaIngreso"), document.getElementById("PlacaIngresoError"), "");
		setInputError(document.getElementById("TipoVehiculoIngreso"), document.getElementById("TipoVehiculoIngresoError"), "");
		setInputError(document.getElementById("EspacioCodigoIngreso"), document.getElementById("EspacioCodigoIngresoError"), "");
		setInputError(document.getElementById("DniClienteIngreso"), document.getElementById("DniClienteIngresoError"), "");
	};

	const validateIngreso = () => {
		clearIngressErrors();

		const placaInput = document.getElementById("PlacaIngreso");
		const tipoInput = document.getElementById("TipoVehiculoIngreso");
		const espacioInput = document.getElementById("EspacioCodigoIngreso");
		const dniInput = document.getElementById("DniClienteIngreso");
		const placaError = document.getElementById("PlacaIngresoError");
		const tipoError = document.getElementById("TipoVehiculoIngresoError");
		const espacioError = document.getElementById("EspacioCodigoIngresoError");
		const dniError = document.getElementById("DniClienteIngresoError");

		const payload = {
			Placa: placaInput?.value.trim().toUpperCase() ?? "",
			TipoVehiculo: tipoInput?.value.trim().toLowerCase() ?? "",
			EspacioCodigo: espacioInput?.value.trim().toUpperCase() ?? "",
			DniCliente: dniInput?.value.trim() ?? ""
		};

		let isValid = true;
		if (!/^[A-Z0-9]{2,4}-[A-Z0-9]{3,4}$/.test(payload.Placa)) {
			setInputError(placaInput, placaError, "La placa no tiene el formato peruano esperado.");
			isValid = false;
		}

		if (!payload.TipoVehiculo) {
			setInputError(tipoInput, tipoError, "El tipo de vehículo es obligatorio.");
			isValid = false;
		}

		if (!payload.EspacioCodigo) {
			setInputError(espacioInput, espacioError, "Debes seleccionar un espacio libre.");
			isValid = false;
		}

		if (!/^[0-9]{8}$/.test(payload.DniCliente)) {
			setInputError(dniInput, dniError, "El DNI debe tener exactamente 8 dígitos.");
			isValid = false;
		}

		if (!isValid) {
			setTextResult("ingresoResult", "Revisa los campos marcados antes de continuar.", true);
			return null;
		}

		return payload;
	};

	const setTextResult = (elementId, message, isError = false) => {
		const element = document.getElementById(elementId);
		if (!element) {
			return;
		}

		element.textContent = message;
		element.style.borderColor = isError ? "#b02a37" : "var(--border-color)";
	};

	const parseJsonResponse = async (response) => {
		const text = await response.text();
		if (!text) {
			return null;
		}

		try {
			return JSON.parse(text);
		} catch {
			return { message: text };
		}
	};

	const writeResult = (selector, value, isError = false) => {
		const element = document.querySelector(selector);
		if (!element) {
			return;
		}

		element.textContent = stringify(value);
		element.style.borderColor = isError ? "#b02a37" : "var(--border-color)";
	};

	const postJsonResponse = async (endpoint, payload, includeToken = false) => {
		const headers = { "Content-Type": "application/json" };
		if (includeToken) {
			const token = getValidToken();
			if (token) {
				headers.Authorization = `Bearer ${token}`;
			} else {
				redirectTo(loginPath);
				throw new Error("Debes iniciar sesión nuevamente.");
			}
		}

		const response = await fetch(endpoint, {
			method: "POST",
			headers,
			body: JSON.stringify(payload)
		});

		return { response, body: await parseJsonResponse(response) };
	};

	const postJson = async (endpoint, payload, includeToken = false) => {
		const { response, body } = await postJsonResponse(endpoint, payload, includeToken);
		if (!response.ok) {
			const errorMessage = body?.message || body?.detail || response.statusText;
			throw new Error(errorMessage);
		}

		return body;
	};

	const bindForm = (formId, endpoint, resultSelector, options = {}) => {
		const form = document.getElementById(formId);
		if (!form) {
			return;
		}

		form.addEventListener("submit", async (event) => {
			event.preventDefault();

			const formData = new FormData(form);
			const payload = Object.fromEntries(formData.entries());

			try {
				const response = await postJson(endpoint, payload, options.authenticated ?? false);
				if (options.saveToken && response?.accessToken) {
					localStorage.setItem(tokenKey, response.accessToken);
					writeResult("#sessionStatus", {
						message: "Token guardado",
						tenantId: response.tenantId,
						role: response.role
					});
					if (options.redirectOnSuccess) {
						redirectTo(dashboardPath);
						return;
					}
				}

				writeResult(resultSelector, response);
			} catch (error) {
				writeResult(resultSelector, { error: error.message }, true);
			}
		});
	};

	const setFieldError = (input, errorElement, message) => {
		if (!input || !errorElement) {
			return;
		}

		input.classList.toggle("is-invalid", Boolean(message));
		input.setAttribute("aria-invalid", String(Boolean(message)));
		errorElement.textContent = message || "";
		errorElement.hidden = !message;
	};

	const setStatusMessage = (element, message, isError = false) => {
		if (!element) {
			return;
		}

		element.textContent = message;
		element.dataset.state = isError ? "error" : "success";
		element.setAttribute("aria-live", isError ? "assertive" : "polite");
		element.style.borderColor = isError ? "#b02a37" : "var(--border-color)";
	};

	const delay = (milliseconds) => new Promise((resolve) => window.setTimeout(resolve, milliseconds));

	const bindLoginForm = () => {
		const form = document.getElementById("loginForm");
		if (!form) {
			return;
		}

		const rucInput = document.getElementById("LoginRuc");
		const usernameInput = document.getElementById("Username");
		const passwordInput = document.getElementById("Password");
		const submitButton = document.getElementById("LoginSubmit");
		const statusBox = document.getElementById("loginResult");
		const rucError = document.getElementById("LoginRucError");
		const usernameError = document.getElementById("UsernameError");
		const passwordError = document.getElementById("PasswordError");
		const passwordToggle = document.getElementById("PasswordToggle");
		let isSubmitting = false;

		const clearFieldErrors = () => {
			setFieldError(rucInput, rucError, "");
			setFieldError(usernameInput, usernameError, "");
			setFieldError(passwordInput, passwordError, "");
		};

		const setSubmittingState = (pending) => {
			if (!submitButton) {
				return;
			}

			submitButton.disabled = pending;
			submitButton.textContent = pending ? "Ingresando..." : (submitButton.dataset.defaultLabel || "Entrar");
			submitButton.setAttribute("aria-busy", String(pending));
		};

		const validate = () => {
			clearFieldErrors();

			const payload = {
				Ruc: rucInput?.value.trim() ?? "",
				Username: usernameInput?.value.trim() ?? "",
				Password: passwordInput?.value ?? ""
			};

			let isValid = true;

			if (!/^[0-9]{11}$/.test(payload.Ruc)) {
				setFieldError(rucInput, rucError, "El RUC debe contener exactamente 11 dígitos numéricos.");
				isValid = false;
			}

			if (!payload.Username) {
				setFieldError(usernameInput, usernameError, "El usuario es obligatorio.");
				isValid = false;
			}

			if (!payload.Password) {
				setFieldError(passwordInput, passwordError, "La contraseña es obligatoria.");
				isValid = false;
			}

			if (!isValid) {
				setStatusMessage(statusBox, "Revisa los campos marcados antes de continuar.", true);
				return null;
			}

			return payload;
		};

		passwordToggle?.addEventListener("click", () => {
			if (!passwordInput) {
				return;
			}

			const reveal = passwordInput.type === "password";
			passwordInput.type = reveal ? "text" : "password";
			passwordToggle.setAttribute("aria-pressed", String(reveal));
			passwordToggle.textContent = reveal ? "Ocultar contraseña" : "Mostrar contraseña";
		});

		[rucInput, usernameInput, passwordInput].forEach((input) => {
			input?.addEventListener("input", () => {
				if (input === rucInput) {
					setFieldError(rucInput, rucError, "");
				}

				if (input === usernameInput) {
					setFieldError(usernameInput, usernameError, "");
				}

				if (input === passwordInput) {
					setFieldError(passwordInput, passwordError, "");
				}
			});
		});

		form.addEventListener("submit", async (event) => {
			event.preventDefault();

			if (isSubmitting) {
				return;
			}

			const payload = validate();
			if (!payload) {
				return;
			}

			isSubmitting = true;
			setSubmittingState(true);

			try {
				const { response, body } = await postJsonResponse("/api/auth/login", payload, false);
				if (response.ok && body?.accessToken) {
					localStorage.setItem(tokenKey, body.accessToken);
					setStatusMessage(statusBox, "Sesión iniciada correctamente. Redirigiendo al Dashboard...");
					writeResult("#sessionStatus", {
						message: "Token guardado",
						tenantId: body.tenantId,
						role: body.role
					});
					await delay(150);
					redirectTo(dashboardPath);
					return;
				}

				if (response.status === 401) {
					setStatusMessage(statusBox, "Credenciales incorrectas. Verifica el RUC, el usuario y la contraseña.", true);
					return;
				}

				if (response.status === 403) {
					setStatusMessage(statusBox, "El usuario está inactivo. Contacta al administrador del tenant.", true);
					return;
				}

				const errorMessage = body?.message || body?.detail || response.statusText || "No se pudo iniciar sesión.";
				setStatusMessage(statusBox, errorMessage, true);
			} catch (error) {
				setStatusMessage(statusBox, error.message || "No se pudo iniciar sesión.", true);
			} finally {
				isSubmitting = false;
				setSubmittingState(false);
			}
		});
	};

	const bindDashboardPage = () => {
		const sessionStatus = document.getElementById("sessionStatus");
		const token = getValidToken();

		if (sessionStatus) {
			sessionStatus.textContent = token ? "Token cargado desde almacenamiento local" : "Sesión no autenticada";
		}
	};

	const bindTarifasPage = () => {
		const form = document.getElementById("tarifasForm");
		if (!form) {
			return;
		}

		const reloadButton = document.getElementById("reloadTarifasButton");
		const confirmButton = document.getElementById("confirmTarifasButton");
		const resultBox = document.getElementById("tarifasResult");
		const history = document.getElementById("tarifasHistory");

		const getRows = () => Array.from(document.querySelectorAll(".tarifa-row"));

		const getPayload = () => getRows().map((row) => {
			const vehicle = row.dataset.vehicle;
			const tarifaHora = Number(row.querySelector('[data-field="tarifaHora"]').value || 0);
			const tarifaDia = Number(row.querySelector('[data-field="tarifaDia"]').value || 0);
			return { tipoVehiculo: vehicle, tarifaHora, tarifaDia };
		});

		const applyTarifas = (tarifas) => {
			tarifas.forEach((tarifa) => {
				const vehicle = tarifa.tipoVehiculo?.toLowerCase?.() ?? tarifa.tipoVehiculo;
				const horaInput = document.querySelector(`.tarifa-row[data-vehicle="${vehicle}"] [data-field="tarifaHora"]`);
				const diaInput = document.querySelector(`.tarifa-row[data-vehicle="${vehicle}"] [data-field="tarifaDia"]`);
				if (horaInput) {
					horaInput.value = formatDecimal(tarifa.tarifaHora);
				}
				if (diaInput) {
					diaInput.value = formatDecimal(tarifa.tarifaDia);
				}
			});
			setTariffSummary(tarifas);
		};

		const loadTarifas = async () => {
			const token = getValidToken();
			if (!token) {
				redirectTo(loginPath);
				return;
			}

			try {
				const response = await fetch("/api/tarifas/current", {
					headers: { Authorization: `Bearer ${token}` }
				});

				if (!response.ok) {
					throw new Error((await response.text()) || response.statusText);
				}

				const data = await response.json();
				applyTarifas(data.tarifas || []);
				setTextResult("tarifasResult", "Tarifas cargadas correctamente.");
				if (history) {
					history.innerHTML = `
						<div class="history-card">
							<strong>Última carga</strong>
							<small>${new Date().toLocaleString("es-PE")}</small>
						</div>`;
				}
			} catch (error) {
				setTextResult("tarifasResult", error.message || "No se pudieron cargar las tarifas.", true);
			}
		};

		const saveTarifas = async () => {
			const token = getValidToken();
			if (!token) {
				redirectTo(loginPath);
				return;
			}

			const tarifas = getPayload();
			if (tarifas.some((tarifa) => Number.isNaN(tarifa.tarifaHora) || Number.isNaN(tarifa.tarifaDia) || tarifa.tarifaHora < 0 || tarifa.tarifaDia < 0)) {
				setTextResult("tarifasResult", "Las tarifas no pueden ser negativas y deben ser numéricas.", true);
				return;
			}

			if (!window.confirm("¿Deseas guardar las nuevas tarifas?")) {
				setTextResult("tarifasResult", "Guardado cancelado por el usuario.");
				return;
			}

			try {
				const response = await fetch("/api/tarifas/current", {
					method: "PUT",
					headers: {
						"Content-Type": "application/json",
						Authorization: `Bearer ${token}`
					},
					body: JSON.stringify({ tarifas })
				});

				const bodyText = await response.text();
				if (!response.ok) {
					throw new Error(bodyText || response.statusText);
				}

				const data = bodyText ? JSON.parse(bodyText) : null;
				applyTarifas(data?.tarifas || tarifas);
				setTextResult("tarifasResult", "Tarifas guardadas correctamente.");
			} catch (error) {
				setTextResult("tarifasResult", error.message || "No se pudieron guardar las tarifas.", true);
			}
		};

		reloadButton?.addEventListener("click", () => loadTarifas());
		confirmButton?.addEventListener("click", () => saveTarifas());
		form.addEventListener("submit", (event) => {
			event.preventDefault();
			saveTarifas();
		});

		loadTarifas();
	};

	const bindIngresoPage = () => {
		const form = document.getElementById("ingresoForm");
		if (!form) {
			return;
		}

		const tipoInput = document.getElementById("TipoVehiculoIngreso");
		const spaceStatus = document.getElementById("espaciosDisponiblesStatus");
		const reloadSpacesButton = document.getElementById("reloadSpacesButton");
		const salidaForm = document.getElementById("salidaForm");
		let lastSelectedSpace = "";

		const loadSpaces = async () => {
			const token = getValidToken();
			if (!token) {
				redirectTo(loginPath);
				return;
			}

			const tipoVehiculo = tipoInput?.value || "";
			if (spaceStatus) {
				spaceStatus.textContent = "Cargando disponibilidad";
			}

			try {
				const response = await fetch(`/api/parking/espacios-libres?tipoVehiculo=${encodeURIComponent(tipoVehiculo)}`, {
					headers: { Authorization: `Bearer ${token}` }
				});

				if (!response.ok) {
					throw new Error((await response.text()) || response.statusText);
				}

				const spaces = await response.json();
				populateSpaceOptions(spaces, lastSelectedSpace);
				if (spaceStatus) {
					spaceStatus.textContent = spaces.length ? `${spaces.length} espacios compatibles` : "Sin espacios compatibles";
				}
			} catch (error) {
				if (spaceStatus) {
					spaceStatus.textContent = "Error al cargar espacios";
				}
				setTextResult("ingresoResult", error.message || "No se pudieron cargar los espacios libres.", true);
			}
		};

		tipoInput?.addEventListener("change", () => {
			lastSelectedSpace = "";
			loadSpaces();
		});

		reloadSpacesButton?.addEventListener("click", () => loadSpaces());

		form.addEventListener("submit", async (event) => {
			event.preventDefault();

			const payload = validateIngreso();
			if (!payload) {
				return;
			}

			try {
				const { response, body } = await postJsonResponse("/api/parking/ingreso", payload, true);
				if (!response.ok) {
					throw new Error(body?.message || response.statusText);
				}

				lastSelectedSpace = payload.EspacioCodigo;
				setTextResult("ingresoResult", `Ingreso registrado para ${body.placa || payload.Placa} en ${body.espacioCodigo || payload.EspacioCodigo}.`);
				await loadSpaces();
			} catch (error) {
				setTextResult("ingresoResult", error.message || "No se pudo registrar el ingreso.", true);
			}
		});

		salidaForm?.addEventListener("submit", async (event) => {
			event.preventDefault();

			const placaInput = document.getElementById("PlacaSalida");
			const placa = placaInput?.value.trim().toUpperCase() ?? "";
			if (!/^[A-Z0-9]{2,4}-[A-Z0-9]{3,4}$/.test(placa)) {
				setTextResult("salidaResult", "La placa no tiene el formato peruano esperado.", true);
				return;
			}

			try {
				const { response, body } = await postJsonResponse("/api/parking/salida", { Placa: placa }, true);
				if (!response.ok) {
					throw new Error(body?.message || response.statusText);
				}

				setTextResult("salidaResult", `Salida liquidada para ${body.placa || placa}. Total: ${formatCurrency(body.total)}`);
				await loadSpaces();
			} catch (error) {
				setTextResult("salidaResult", error.message || "No se pudo liquidar la salida.", true);
			}
		});

		loadSpaces();
	};

	document.addEventListener("DOMContentLoaded", () => {
		if (enforceRouteGuard()) {
			return;
		}

		syncTheme();
		bindDashboardPage();

		document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
			button.addEventListener("click", (event) => {
				event.preventDefault();
				const nextTheme = document.body.getAttribute("data-theme") === "dark" ? "light" : "dark";
				setTheme(nextTheme);
			});
		});

		bindForm("registerForm", "/api/auth/register", "#registerResult");
		bindLoginForm();
		bindTarifasPage();
		bindIngresoPage();

		const token = getValidToken();
		if (token) {
			writeResult("#sessionStatus", { message: "Token cargado desde almacenamiento local" });
		} else {
			writeResult("#sessionStatus", { message: "Sesión no autenticada" });
		}
	});
})();
